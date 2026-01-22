import React, { useState, useEffect } from 'react';
import { View, Text, StyleSheet, ScrollView, TouchableOpacity, Image, Alert, Platform } from 'react-native';
import MapView, { Marker, Polyline } from 'react-native-maps';
import { useUser } from '../context/UserContext';
import { rideService, bannerService } from '../services/api';
import signalRService from '../services/signalr';

export default function RideFlowScreen({ navigation }) {
  const { user } = useUser();
  const [ride, setRide] = useState(null);
  const [banners, setBanners] = useState([]);
  const [driverLocation, setDriverLocation] = useState(null);
  const [estimatedPrice, setEstimatedPrice] = useState(null);
  const [destination, setDestination] = useState(null);

  const passengerLocation = { latitude: 40.748817, longitude: -73.985428 };

  useEffect(() => {
    loadBanners();
    connectSignalR();
    
    return () => {
      signalRService.disconnect();
    };
  }, []);

  useEffect(() => {
    if (ride) {
      signalRService.joinRideGroup(ride.id);
    }
  }, [ride]);

  const loadBanners = async () => {
    try {
      const data = await bannerService.getBanners();
      setBanners(data);
    } catch (error) {
      console.error('Error loading banners:', error);
    }
  };

  const connectSignalR = async () => {
    await signalRService.connect();
    
    signalRService.on('RideAccepted', (data) => {
      if (ride && data.rideId === ride.id) {
        setRide(prev => ({ ...prev, status: 'ACCEPTED', driverId: data.driverId }));
        Alert.alert('Driver Found!', 'Your driver is on the way');
      }
    });

    signalRService.on('RideStatusChanged', (data) => {
      if (ride && data.rideId === ride.id) {
        setRide(prev => ({ ...prev, status: data.status }));
      }
    });

    signalRService.on('DriverLocationUpdated', (data) => {
      setDriverLocation({
        latitude: parseFloat(data.latitude),
        longitude: parseFloat(data.longitude)
      });
    });

    signalRService.on('RideCompleted', (data) => {
      if (ride && data.rideId === ride.id) {
        Alert.alert('Ride Completed', 'Thank you for riding with us!');
        setRide(null);
        setDestination(null);
        setEstimatedPrice(null);
      }
    });
  };

  const handleMapPress = async (event) => {
    const { latitude, longitude } = event.nativeEvent.coordinate;
    setDestination({ latitude, longitude });

    try {
      const estimate = await rideService.estimatePrice(
        passengerLocation.latitude,
        passengerLocation.longitude,
        latitude,
        longitude
      );
      setEstimatedPrice(estimate);
    } catch (error) {
      console.error('Error estimating price:', error);
    }
  };

  const handleRequestRide = async () => {
    if (!destination || !user) return;

    try {
      const newRide = await rideService.createRide(
        user.id,
        passengerLocation.latitude,
        passengerLocation.longitude,
        destination.latitude,
        destination.longitude,
        'Current Location',
        'Selected Destination'
      );
      setRide(newRide);
      Alert.alert('Searching...', 'Looking for nearby drivers');
    } catch (error) {
      Alert.alert('Error', 'Failed to create ride');
      console.error('Error creating ride:', error);
    }
  };

  const handleCancelRide = async () => {
    if (!ride) return;

    try {
      await rideService.cancelRide(ride.id);
      setRide(null);
      setDestination(null);
      setEstimatedPrice(null);
      Alert.alert('Cancelled', 'Your ride has been cancelled');
    } catch (error) {
      Alert.alert('Error', 'Failed to cancel ride');
    }
  };

  return (
    <View style={styles.container}>
      {banners.length > 0 && (
        <ScrollView horizontal style={styles.bannerContainer} showsHorizontalScrollIndicator={false}>
          {banners.map(banner => (
            <Image
              key={banner.id}
              source={{ uri: banner.imageUrl }}
              style={styles.banner}
              resizeMode="cover"
            />
          ))}
        </ScrollView>
      )}

      <MapView
        style={styles.map}
        initialRegion={{
          latitude: passengerLocation.latitude,
          longitude: passengerLocation.longitude,
          latitudeDelta: 0.05,
          longitudeDelta: 0.05,
        }}
        onPress={!ride ? handleMapPress : null}
      >
        <Marker coordinate={passengerLocation} title="You" pinColor="blue" />
        
        {destination && <Marker coordinate={destination} title="Destination" pinColor="red" />}
        
        {driverLocation && ride && (
          <Marker coordinate={driverLocation} title="Driver" pinColor="green" />
        )}

        {destination && (
          <Polyline
            coordinates={[passengerLocation, destination]}
            strokeColor="#667eea"
            strokeWidth={3}
          />
        )}
      </MapView>

      <View style={styles.bottomSheet}>
        {!ride && (
          <>
            <Text style={styles.title}>Where to?</Text>
            <Text style={styles.instruction}>Tap on the map to select your destination</Text>
            
            {estimatedPrice && (
              <View style={styles.priceContainer}>
                <Text style={styles.priceLabel}>Estimated Price</Text>
                <Text style={styles.price}>${estimatedPrice.estimatedPrice.toFixed(2)}</Text>
                <Text style={styles.distance}>{estimatedPrice.distance.toFixed(2)} km</Text>
              </View>
            )}

            {destination && (
              <TouchableOpacity style={styles.requestButton} onPress={handleRequestRide}>
                <Text style={styles.requestButtonText}>Request Ride</Text>
              </TouchableOpacity>
            )}
          </>
        )}

        {ride && (
          <>
            <Text style={styles.title}>
              {ride.status === 'SEARCHING' && 'Searching for driver...'}
              {ride.status === 'ACCEPTED' && 'Driver Accepted'}
              {ride.status === 'IN_PROGRESS' && 'Ride in Progress'}
            </Text>
            
            <View style={styles.rideInfo}>
              <Text style={styles.ridePrice}>${ride.price.toFixed(2)}</Text>
              <Text style={styles.rideStatus}>{ride.status}</Text>
            </View>

            {ride.status === 'SEARCHING' && (
              <TouchableOpacity style={styles.cancelButton} onPress={handleCancelRide}>
                <Text style={styles.cancelButtonText}>Cancel</Text>
              </TouchableOpacity>
            )}
          </>
        )}
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
  bannerContainer: {
    position: 'absolute',
    top: 50,
    left: 0,
    right: 0,
    zIndex: 10,
    height: 120,
    paddingHorizontal: 10,
  },
  banner: {
    width: 300,
    height: 100,
    borderRadius: 10,
    marginRight: 10,
  },
  map: {
    flex: 1,
  },
  bottomSheet: {
    position: 'absolute',
    bottom: 0,
    left: 0,
    right: 0,
    backgroundColor: 'white',
    borderTopLeftRadius: 20,
    borderTopRightRadius: 20,
    padding: 20,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: -2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 5,
  },
  title: {
    fontSize: 22,
    fontWeight: 'bold',
    marginBottom: 10,
  },
  instruction: {
    fontSize: 14,
    color: '#666',
    marginBottom: 15,
  },
  priceContainer: {
    backgroundColor: '#f5f5f5',
    padding: 15,
    borderRadius: 10,
    marginBottom: 15,
  },
  priceLabel: {
    fontSize: 12,
    color: '#666',
    marginBottom: 5,
  },
  price: {
    fontSize: 28,
    fontWeight: 'bold',
    color: '#667eea',
  },
  distance: {
    fontSize: 14,
    color: '#666',
    marginTop: 5,
  },
  requestButton: {
    backgroundColor: '#667eea',
    padding: 18,
    borderRadius: 10,
    alignItems: 'center',
  },
  requestButtonText: {
    color: 'white',
    fontSize: 18,
    fontWeight: 'bold',
  },
  rideInfo: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 15,
  },
  ridePrice: {
    fontSize: 24,
    fontWeight: 'bold',
    color: '#667eea',
  },
  rideStatus: {
    fontSize: 14,
    color: '#666',
    backgroundColor: '#f0f0f0',
    paddingHorizontal: 10,
    paddingVertical: 5,
    borderRadius: 5,
  },
  cancelButton: {
    backgroundColor: '#ff4444',
    padding: 18,
    borderRadius: 10,
    alignItems: 'center',
  },
  cancelButtonText: {
    color: 'white',
    fontSize: 16,
    fontWeight: 'bold',
  },
});

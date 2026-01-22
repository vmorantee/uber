import React, { useState, useEffect } from 'react';
import { View, Text, StyleSheet, TouchableOpacity, Alert, Switch, Platform } from 'react-native';
import MapView, { Marker } from 'react-native-maps';
import { useUser } from '../context/UserContext';
import { rideService } from '../services/api';
import signalRService from '../services/signalr';

export default function DriverScreen({ navigation }) {
  const { user } = useUser();
  const [isOnline, setIsOnline] = useState(false);
  const [currentLocation, setCurrentLocation] = useState({ latitude: 40.750000, longitude: -73.990000 });
  const [pendingRide, setPendingRide] = useState(null);
  const [activeRide, setActiveRide] = useState(null);

  useEffect(() => {
    connectSignalR();
    
    return () => {
      signalRService.disconnect();
    };
  }, []);

  useEffect(() => {
    if (isOnline && user) {
      updateLocation();
      const interval = setInterval(updateLocation, 5000);
      return () => clearInterval(interval);
    }
  }, [isOnline]);

  const connectSignalR = async () => {
    await signalRService.connect();
    
    signalRService.on('NewRideRequest', (data) => {
      if (isOnline && !activeRide) {
        setPendingRide(data);
        Alert.alert(
          'New Ride Request',
          `Price: $${data.price}\nDistance: ${data.distance} km`,
          [
            { text: 'Decline', onPress: () => setPendingRide(null), style: 'cancel' },
            { text: 'Accept', onPress: () => handleAcceptRide(data.rideId) }
          ]
        );
      }
    });
  };

  const updateLocation = async () => {
    if (!user) return;

    try {
      await rideService.updateDriverLocation(user.id, currentLocation.latitude, currentLocation.longitude, isOnline);
    } catch (error) {
      console.error('Error updating location:', error);
    }
  };

  const handleGoOnline = async () => {
    setIsOnline(true);
    await updateLocation();
    Alert.alert('You are now online', 'You will receive ride requests');
  };

  const handleGoOffline = async () => {
    setIsOnline(false);
    if (user) {
      await rideService.updateDriverLocation(user.id, currentLocation.latitude, currentLocation.longitude, false);
    }
    Alert.alert('You are now offline', 'You will not receive ride requests');
  };

  const handleAcceptRide = async (rideId) => {
    if (!user) return;

    try {
      const ride = await rideService.acceptRide(rideId, user.id);
      setActiveRide(ride);
      setPendingRide(null);
      await signalRService.joinRideGroup(rideId);
      Alert.alert('Ride Accepted', 'Navigate to passenger location');
    } catch (error) {
      Alert.alert('Error', 'Failed to accept ride');
      console.error('Error accepting ride:', error);
    }
  };

  const handleStartRide = async () => {
    if (!activeRide) return;

    try {
      const ride = await rideService.startRide(activeRide.id);
      setActiveRide(ride);
      Alert.alert('Ride Started', 'Take passenger to destination');
    } catch (error) {
      Alert.alert('Error', 'Failed to start ride');
    }
  };

  const handleCompleteRide = async () => {
    if (!activeRide) return;

    try {
      await rideService.completeRide(activeRide.id);
      Alert.alert('Ride Completed', `You earned $${(activeRide.price * 0.8).toFixed(2)}`);
      setActiveRide(null);
    } catch (error) {
      Alert.alert('Error', error.response?.data || 'Failed to complete ride');
      console.error('Error completing ride:', error);
    }
  };

  return (
    <View style={styles.container}>
      <MapView
        style={styles.map}
        initialRegion={{
          latitude: currentLocation.latitude,
          longitude: currentLocation.longitude,
          latitudeDelta: 0.05,
          longitudeDelta: 0.05,
        }}
      >
        <Marker coordinate={currentLocation} title="You" pinColor="green" />
        
        {activeRide && (
          <>
            <Marker
              coordinate={{
                latitude: parseFloat(activeRide.startLocationLat),
                longitude: parseFloat(activeRide.startLocationLng)
              }}
              title="Pickup"
              pinColor="blue"
            />
            <Marker
              coordinate={{
                latitude: parseFloat(activeRide.endLocationLat),
                longitude: parseFloat(activeRide.endLocationLng)
              }}
              title="Dropoff"
              pinColor="red"
            />
          </>
        )}
      </MapView>

      <View style={styles.controlPanel}>
        <View style={styles.statusContainer}>
          <Text style={styles.statusLabel}>Driver Status</Text>
          <View style={styles.switchContainer}>
            <Text style={styles.switchLabel}>{isOnline ? 'Online' : 'Offline'}</Text>
            <Switch
              value={isOnline}
              onValueChange={(value) => value ? handleGoOnline() : handleGoOffline()}
              trackColor={{ false: '#ccc', true: '#667eea' }}
              thumbColor={isOnline ? '#4c51bf' : '#f4f3f4'}
            />
          </View>
        </View>

        {!activeRide && isOnline && (
          <View style={styles.waitingContainer}>
            <Text style={styles.waitingText}>Waiting for ride requests...</Text>
          </View>
        )}

        {activeRide && (
          <View style={styles.rideContainer}>
            <Text style={styles.rideTitle}>Active Ride</Text>
            <View style={styles.rideDetails}>
              <Text style={styles.ridePrice}>${activeRide.price.toFixed(2)}</Text>
              <Text style={styles.rideStatus}>{activeRide.status}</Text>
            </View>

            {activeRide.status === 'ACCEPTED' && (
              <TouchableOpacity style={styles.actionButton} onPress={handleStartRide}>
                <Text style={styles.actionButtonText}>Start Ride</Text>
              </TouchableOpacity>
            )}

            {activeRide.status === 'IN_PROGRESS' && (
              <TouchableOpacity style={styles.actionButton} onPress={handleCompleteRide}>
                <Text style={styles.actionButtonText}>Complete Ride</Text>
              </TouchableOpacity>
            )}
          </View>
        )}
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
  map: {
    flex: 1,
  },
  controlPanel: {
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
  statusContainer: {
    marginBottom: 15,
  },
  statusLabel: {
    fontSize: 16,
    fontWeight: '600',
    marginBottom: 10,
  },
  switchContainer: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    backgroundColor: '#f5f5f5',
    padding: 15,
    borderRadius: 10,
  },
  switchLabel: {
    fontSize: 18,
    fontWeight: 'bold',
    color: '#333',
  },
  waitingContainer: {
    padding: 20,
    alignItems: 'center',
  },
  waitingText: {
    fontSize: 16,
    color: '#666',
    fontStyle: 'italic',
  },
  rideContainer: {
    backgroundColor: '#f9f9f9',
    padding: 15,
    borderRadius: 10,
  },
  rideTitle: {
    fontSize: 18,
    fontWeight: 'bold',
    marginBottom: 10,
  },
  rideDetails: {
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
    backgroundColor: '#e0e0e0',
    paddingHorizontal: 10,
    paddingVertical: 5,
    borderRadius: 5,
  },
  actionButton: {
    backgroundColor: '#667eea',
    padding: 18,
    borderRadius: 10,
    alignItems: 'center',
  },
  actionButtonText: {
    color: 'white',
    fontSize: 16,
    fontWeight: 'bold',
  },
});

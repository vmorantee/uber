import React from 'react';
import { View, Text, StyleSheet, Image, TouchableOpacity, Alert } from 'react-native';
import { useUser } from '../context/UserContext';

export default function ProfileScreen({ navigation }) {
  const { user, switchContext, logout } = useUser();

  if (!user) return null;

  const handleSwitchContext = async () => {
    if (user.currentContext === 'Driver') {
      await switchContext('Passenger');
      Alert.alert('Switched to Passenger Mode');
    } else if (user.isDriverApproved) {
      await switchContext('Driver');
      Alert.alert('Switched to Driver Mode');
      navigation.navigate('Driver');
    } else {
      Alert.alert('Not Approved', 'You are not approved as a driver yet');
    }
  };

  const handleLogout = async () => {
    Alert.alert(
      'Logout',
      'Are you sure you want to logout?',
      [
        { text: 'Cancel', style: 'cancel' },
        {
          text: 'Logout',
          style: 'destructive',
          onPress: async () => {
            await logout();
            navigation.replace('Login');
          }
        }
      ]
    );
  };

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <View style={styles.avatarContainer}>
          <Text style={styles.avatarText}>
            {user.firstName.charAt(0)}{user.lastName.charAt(0)}
          </Text>
        </View>
        <Text style={styles.name}>{user.firstName} {user.lastName}</Text>
        <Text style={styles.email}>{user.email}</Text>
        <View style={styles.badge}>
          <Text style={styles.badgeText}>{user.role}</Text>
        </View>
      </View>

      <View style={styles.infoCard}>
        <View style={styles.infoRow}>
          <Text style={styles.infoLabel}>Current Mode</Text>
          <Text style={styles.infoValue}>{user.currentContext}</Text>
        </View>
        <View style={styles.infoRow}>
          <Text style={styles.infoLabel}>Wallet Balance</Text>
          <Text style={styles.infoValue}>${user.walletBalance?.toFixed(2) || '0.00'}</Text>
        </View>
        {user.role === 'Driver' && (
          <>
            <View style={styles.infoRow}>
              <Text style={styles.infoLabel}>Driver Status</Text>
              <Text style={[styles.infoValue, { color: user.isDriverApproved ? '#4caf50' : '#ff9800' }]}>
                {user.isDriverApproved ? 'Approved' : 'Pending'}
              </Text>
            </View>
            {user.isDriverApproved && (
              <View style={styles.infoRow}>
                <Text style={styles.infoLabel}>Vehicle</Text>
                <Text style={styles.infoValue}>{user.vehicleModel || 'N/A'}</Text>
              </View>
            )}
          </>
        )}
      </View>

      {user.role === 'Driver' && user.isDriverApproved && (
        <TouchableOpacity style={styles.switchButton} onPress={handleSwitchContext}>
          <Text style={styles.switchButtonText}>
            Switch to {user.currentContext === 'Driver' ? 'Passenger' : 'Driver'} Mode
          </Text>
        </TouchableOpacity>
      )}

      <TouchableOpacity style={styles.walletButton} onPress={() => navigation.navigate('Wallet')}>
        <Text style={styles.walletButtonText}>💳 Manage Wallet</Text>
      </TouchableOpacity>

      <TouchableOpacity style={styles.logoutButton} onPress={handleLogout}>
        <Text style={styles.logoutButtonText}>Logout</Text>
      </TouchableOpacity>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f5f5f5',
  },
  header: {
    backgroundColor: '#667eea',
    padding: 30,
    alignItems: 'center',
    paddingTop: 60,
  },
  avatarContainer: {
    width: 100,
    height: 100,
    borderRadius: 50,
    backgroundColor: 'rgba(255,255,255,0.3)',
    justifyContent: 'center',
    alignItems: 'center',
    marginBottom: 15,
  },
  avatarText: {
    fontSize: 36,
    fontWeight: 'bold',
    color: 'white',
  },
  name: {
    fontSize: 24,
    fontWeight: 'bold',
    color: 'white',
    marginBottom: 5,
  },
  email: {
    fontSize: 14,
    color: 'rgba(255,255,255,0.8)',
    marginBottom: 10,
  },
  badge: {
    backgroundColor: 'rgba(255,255,255,0.2)',
    paddingHorizontal: 15,
    paddingVertical: 5,
    borderRadius: 15,
  },
  badgeText: {
    color: 'white',
    fontWeight: '600',
  },
  infoCard: {
    backgroundColor: 'white',
    margin: 15,
    padding: 20,
    borderRadius: 15,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 2,
  },
  infoRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    paddingVertical: 12,
    borderBottomWidth: 1,
    borderBottomColor: '#f0f0f0',
  },
  infoLabel: {
    fontSize: 16,
    color: '#666',
  },
  infoValue: {
    fontSize: 16,
    fontWeight: '600',
    color: '#333',
  },
  switchButton: {
    backgroundColor: '#667eea',
    margin: 15,
    marginTop: 0,
    padding: 18,
    borderRadius: 10,
    alignItems: 'center',
  },
  switchButtonText: {
    color: 'white',
    fontSize: 16,
    fontWeight: 'bold',
  },
  walletButton: {
    backgroundColor: 'white',
    margin: 15,
    marginTop: 0,
    padding: 18,
    borderRadius: 10,
    alignItems: 'center',
    borderWidth: 2,
    borderColor: '#667eea',
  },
  walletButtonText: {
    color: '#667eea',
    fontSize: 16,
    fontWeight: 'bold',
  },
  logoutButton: {
    backgroundColor: '#ff4444',
    margin: 15,
    marginTop: 'auto',
    marginBottom: 30,
    padding: 18,
    borderRadius: 10,
    alignItems: 'center',
  },
  logoutButtonText: {
    color: 'white',
    fontSize: 16,
    fontWeight: 'bold',
  },
});

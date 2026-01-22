import React from 'react';
import { NavigationContainer } from '@react-navigation/native';
import { createStackNavigator } from '@react-navigation/stack';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import { UserProvider, useUser } from './context/UserContext';

import LoginScreen from './screens/LoginScreen';
import RideFlowScreen from './screens/RideFlowScreen';
import DriverScreen from './screens/DriverScreen';
import ProfileScreen from './screens/ProfileScreen';
import WalletScreen from './screens/WalletScreen';

const Stack = createStackNavigator();
const Tab = createBottomTabNavigator();

function PassengerTabs() {
  return (
    <Tab.Navigator screenOptions={{ headerShown: false }}>
      <Tab.Screen 
        name="Home" 
        component={RideFlowScreen}
        options={{
          tabBarLabel: 'Ride',
          tabBarIcon: () => '🚗',
        }}
      />
      <Tab.Screen 
        name="Wallet" 
        component={WalletScreen}
        options={{
          tabBarLabel: 'Wallet',
          tabBarIcon: () => '💳',
        }}
      />
      <Tab.Screen 
        name="Profile" 
        component={ProfileScreen}
        options={{
          tabBarLabel: 'Profile',
          tabBarIcon: () => '👤',
        }}
      />
    </Tab.Navigator>
  );
}

function DriverTabs() {
  return (
    <Tab.Navigator screenOptions={{ headerShown: false }}>
      <Tab.Screen 
        name="Driver" 
        component={DriverScreen}
        options={{
          tabBarLabel: 'Drive',
          tabBarIcon: () => '🚕',
        }}
      />
      <Tab.Screen 
        name="Wallet" 
        component={WalletScreen}
        options={{
          tabBarLabel: 'Wallet',
          tabBarIcon: () => '💳',
        }}
      />
      <Tab.Screen 
        name="Profile" 
        component={ProfileScreen}
        options={{
          tabBarLabel: 'Profile',
          tabBarIcon: () => '👤',
        }}
      />
    </Tab.Navigator>
  );
}

function MainNavigator() {
  const { user } = useUser();

  if (!user) {
    return null;
  }

  return user.currentContext === 'Driver' ? <DriverTabs /> : <PassengerTabs />;
}

function AppNavigator() {
  const { user, loading } = useUser();

  if (loading) {
    return null;
  }

  return (
    <NavigationContainer>
      <Stack.Navigator screenOptions={{ headerShown: false }}>
        {!user ? (
          <Stack.Screen name="Login" component={LoginScreen} />
        ) : (
          <Stack.Screen name="Main" component={MainNavigator} />
        )}
      </Stack.Navigator>
    </NavigationContainer>
  );
}

export default function App() {
  return (
    <UserProvider>
      <AppNavigator />
    </UserProvider>
  );
}

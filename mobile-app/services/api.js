import axios from 'axios';
import AsyncStorage from '@react-native-async-storage/async-storage';

const API_BASE_URL = 'http://192.168.1.13:5000/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

api.interceptors.request.use(async (config) => {
  const token = await AsyncStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export const authService = {
  login: async (email, password) => {
    const response = await api.post('/auth/login', { email, password });
    if (response.data.token) {
      await AsyncStorage.setItem('token', response.data.token);
      await AsyncStorage.setItem('user', JSON.stringify(response.data.user));
    }
    return response.data;
  },

  register: async (userData) => {
    const response = await api.post('/auth/register', userData);
    if (response.data.token) {
      await AsyncStorage.setItem('token', response.data.token);
      await AsyncStorage.setItem('user', JSON.stringify(response.data.user));
    }
    return response.data;
  },

  logout: async () => {
    await AsyncStorage.removeItem('token');
    await AsyncStorage.removeItem('user');
  },

  getCurrentUser: async () => {
    const userStr = await AsyncStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
  },

  updateContext: async (userId, newContext) => {
    const response = await api.put(`/auth/user/${userId}/context`, JSON.stringify(newContext), {
      headers: { 'Content-Type': 'application/json' }
    });
    await AsyncStorage.setItem('user', JSON.stringify(response.data));
    return response.data;
  }
};

export const rideService = {
  estimatePrice: async (startLat, startLng, endLat, endLng) => {
    const response = await api.post('/ride/estimate', {
      startLocationLat: startLat,
      startLocationLng: startLng,
      endLocationLat: endLat,
      endLocationLng: endLng
    });
    return response.data;
  },

  createRide: async (passengerId, startLat, startLng, endLat, endLng, startAddress, endAddress) => {
    const response = await api.post(`/ride?passengerId=${passengerId}`, {
      startLocationLat: startLat,
      startLocationLng: startLng,
      endLocationLat: endLat,
      endLocationLng: endLng,
      startAddress,
      endAddress
    });
    return response.data;
  },

  acceptRide: async (rideId, driverId) => {
    const response = await api.post(`/ride/${rideId}/accept?driverId=${driverId}`);
    return response.data;
  },

  startRide: async (rideId) => {
    const response = await api.post(`/ride/${rideId}/start`);
    return response.data;
  },

  completeRide: async (rideId) => {
    const response = await api.post(`/ride/${rideId}/complete`);
    return response.data;
  },

  cancelRide: async (rideId) => {
    const response = await api.post(`/ride/${rideId}/cancel`);
    return response.data;
  },

  getRide: async (rideId) => {
    const response = await api.get(`/ride/${rideId}`);
    return response.data;
  },

  getPassengerRides: async (passengerId) => {
    const response = await api.get(`/ride/passenger/${passengerId}`);
    return response.data;
  },

  getDriverRides: async (driverId) => {
    const response = await api.get(`/ride/driver/${driverId}`);
    return response.data;
  },

  updateDriverLocation: async (driverId, latitude, longitude, isOnline) => {
    await api.post(`/ride/location/update?driverId=${driverId}`, {
      latitude,
      longitude,
      isOnline
    });
  }
};

export const walletService = {
  topUp: async (userId, amount) => {
    const response = await api.post(`/wallet/topup?userId=${userId}`, { amount });
    return response.data;
  },

  getBalance: async (userId) => {
    const response = await api.get(`/wallet/balance/${userId}`);
    return response.data;
  },

  getTransactions: async (userId) => {
    const response = await api.get(`/wallet/transactions/${userId}`);
    return response.data;
  }
};

export const bannerService = {
  getBanners: async () => {
    const response = await api.get('/auth/banners');
    return response.data;
  }
};

export default api;

import * as signalR from '@microsoft/signalr';

const HUB_URL = 'http://192.168.1.13:5000/hubs/ride';

class SignalRService {
  constructor() {
    this.connection = null;
    this.listeners = {};
  }

  async connect() {
    if (this.connection) {
      return;
    }

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(HUB_URL)
      .withAutomaticReconnect()
      .build();

    this.connection.on('RideStatusChanged', (data) => {
      this.emit('RideStatusChanged', data);
    });

    this.connection.on('DriverLocationUpdated', (data) => {
      this.emit('DriverLocationUpdated', data);
    });

    this.connection.on('NewRideRequest', (data) => {
      this.emit('NewRideRequest', data);
    });

    this.connection.on('RideAccepted', (data) => {
      this.emit('RideAccepted', data);
    });

    this.connection.on('RideCompleted', (data) => {
      this.emit('RideCompleted', data);
    });

    try {
      await this.connection.start();
      console.log('SignalR Connected');
    } catch (err) {
      console.error('SignalR Connection Error:', err);
    }
  }

  async disconnect() {
    if (this.connection) {
      await this.connection.stop();
      this.connection = null;
    }
  }

  async joinRideGroup(rideId) {
    if (this.connection) {
      await this.connection.invoke('JoinRideGroup', rideId);
    }
  }

  async leaveRideGroup(rideId) {
    if (this.connection) {
      await this.connection.invoke('LeaveRideGroup', rideId);
    }
  }

  async joinDriverChannel(driverId) {
    if (this.connection) {
      await this.connection.invoke('JoinDriverChannel', driverId);
    }
  }

  on(event, callback) {
    if (!this.listeners[event]) {
      this.listeners[event] = [];
    }
    this.listeners[event].push(callback);
  }

  off(event, callback) {
    if (this.listeners[event]) {
      this.listeners[event] = this.listeners[event].filter(cb => cb !== callback);
    }
  }

  emit(event, data) {
    if (this.listeners[event]) {
      this.listeners[event].forEach(callback => callback(data));
    }
  }
}

export default new SignalRService();

import React, { createContext, useState, useContext, useEffect } from 'react';
import { authService } from '../services/api';

const UserContext = createContext();

export const UserProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadUser();
  }, []);

  const loadUser = async () => {
    try {
      const currentUser = await authService.getCurrentUser();
      setUser(currentUser);
    } catch (error) {
      console.error('Error loading user:', error);
    } finally {
      setLoading(false);
    }
  };

  const login = async (email, password) => {
    const response = await authService.login(email, password);
    setUser(response.user);
    return response;
  };

  const register = async (userData) => {
    const response = await authService.register(userData);
    setUser(response.user);
    return response;
  };

  const logout = async () => {
    await authService.logout();
    setUser(null);
  };

  const switchContext = async (newContext) => {
    if (!user) return;
    
    const updatedUser = await authService.updateContext(user.id, newContext);
    setUser(updatedUser);
  };

  const refreshUser = async () => {
    await loadUser();
  };

  return (
    <UserContext.Provider value={{ 
      user, 
      loading, 
      login, 
      register, 
      logout, 
      switchContext,
      refreshUser 
    }}>
      {children}
    </UserContext.Provider>
  );
};

export const useUser = () => {
  const context = useContext(UserContext);
  if (!context) {
    throw new Error('useUser must be used within UserProvider');
  }
  return context;
};

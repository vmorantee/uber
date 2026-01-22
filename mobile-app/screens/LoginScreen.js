import React, { useState } from 'react';
import { View, Text, TextInput, TouchableOpacity, StyleSheet, Alert, KeyboardAvoidingView, Platform } from 'react-native';
import { useUser } from '../context/UserContext';

export default function LoginScreen({ navigation }) {
  const { login } = useUser();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);

  const handleLogin = async () => {
    if (!email || !password) {
      Alert.alert('Error', 'Please fill in all fields');
      return;
    }

    setLoading(true);
    try {
      await login(email, password);
      navigation.replace('Main');
    } catch (error) {
      Alert.alert('Login Failed', error.response?.data || 'Invalid credentials');
    } finally {
      setLoading(false);
    }
  };

  return (
    <KeyboardAvoidingView
      behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
      style={styles.container}
    >
      <View style={styles.header}>
        <Text style={styles.logo}>🚗</Text>
        <Text style={styles.title}>RideSharing</Text>
        <Text style={styles.subtitle}>Your ride, your way</Text>
      </View>

      <View style={styles.form}>
        <TextInput
          style={styles.input}
          placeholder="Email"
          value={email}
          onChangeText={setEmail}
          keyboardType="email-address"
          autoCapitalize="none"
        />
        <TextInput
          style={styles.input}
          placeholder="Password"
          value={password}
          onChangeText={setPassword}
          secureTextEntry
        />

        <TouchableOpacity
          style={styles.loginButton}
          onPress={handleLogin}
          disabled={loading}
        >
          <Text style={styles.loginButtonText}>
            {loading ? 'Logging in...' : 'Login'}
          </Text>
        </TouchableOpacity>

        <TouchableOpacity onPress={() => navigation.navigate('Register')}>
          <Text style={styles.registerLink}>
            Don't have an account? <Text style={styles.registerLinkBold}>Sign Up</Text>
          </Text>
        </TouchableOpacity>

        <View style={styles.demoSection}>
          <Text style={styles.demoTitle}>Demo Accounts:</Text>
          <TouchableOpacity onPress={() => { setEmail('john.doe@example.com'); setPassword('HASHED_PASSWORD_123'); }}>
            <Text style={styles.demoText}>Passenger: john.doe@example.com</Text>
          </TouchableOpacity>
          <TouchableOpacity onPress={() => { setEmail('mike.driver@example.com'); setPassword('HASHED_PASSWORD_789'); }}>
            <Text style={styles.demoText}>Driver: mike.driver@example.com</Text>
          </TouchableOpacity>
        </View>
      </View>
    </KeyboardAvoidingView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#667eea',
  },
  header: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    paddingTop: 60,
  },
  logo: {
    fontSize: 80,
    marginBottom: 20,
  },
  title: {
    fontSize: 36,
    fontWeight: 'bold',
    color: 'white',
    marginBottom: 10,
  },
  subtitle: {
    fontSize: 16,
    color: 'rgba(255,255,255,0.8)',
  },
  form: {
    backgroundColor: 'white',
    borderTopLeftRadius: 30,
    borderTopRightRadius: 30,
    padding: 30,
    paddingTop: 40,
  },
  input: {
    borderWidth: 1,
    borderColor: '#ddd',
    borderRadius: 10,
    padding: 15,
    marginBottom: 15,
    fontSize: 16,
  },
  loginButton: {
    backgroundColor: '#667eea',
    padding: 18,
    borderRadius: 10,
    alignItems: 'center',
    marginTop: 10,
  },
  loginButtonText: {
    color: 'white',
    fontSize: 18,
    fontWeight: 'bold',
  },
  registerLink: {
    textAlign: 'center',
    marginTop: 20,
    color: '#666',
  },
  registerLinkBold: {
    color: '#667eea',
    fontWeight: 'bold',
  },
  demoSection: {
    marginTop: 30,
    padding: 15,
    backgroundColor: '#f9f9f9',
    borderRadius: 10,
  },
  demoTitle: {
    fontWeight: 'bold',
    marginBottom: 10,
    color: '#333',
  },
  demoText: {
    color: '#667eea',
    paddingVertical: 5,
  },
});

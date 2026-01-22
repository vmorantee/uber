import React, { useState, useEffect } from 'react';
import { View, Text, StyleSheet, TouchableOpacity, ScrollView, TextInput, Alert } from 'react-native';
import { useUser } from '../context/UserContext';
import { walletService } from '../services/api';

export default function WalletScreen({ navigation }) {
  const { user, refreshUser } = useUser();
  const [balance, setBalance] = useState(0);
  const [transactions, setTransactions] = useState([]);
  const [topUpAmount, setTopUpAmount] = useState('');

  useEffect(() => {
    if (user) {
      loadWalletData();
    }
  }, [user]);

  const loadWalletData = async () => {
    if (!user) return;

    try {
      const balanceData = await walletService.getBalance(user.id);
      setBalance(balanceData.balance);

      const transactionsData = await walletService.getTransactions(user.id);
      setTransactions(transactionsData);
    } catch (error) {
      console.error('Error loading wallet:', error);
    }
  };

  const handleTopUp = async () => {
    const amount = parseFloat(topUpAmount);
    
    if (isNaN(amount) || amount <= 0) {
      Alert.alert('Invalid Amount', 'Please enter a valid amount');
      return;
    }

    if (!user) return;

    try {
      await walletService.topUp(user.id, amount);
      setTopUpAmount('');
      await loadWalletData();
      await refreshUser();
      Alert.alert('Success', `Added $${amount.toFixed(2)} to your wallet`);
    } catch (error) {
      Alert.alert('Error', 'Failed to top up wallet');
      console.error('Error topping up:', error);
    }
  };

  const getTransactionColor = (type) => {
    switch (type) {
      case 'TopUp':
        return '#4caf50';
      case 'Earning':
        return '#2196f3';
      case 'RidePayment':
        return '#ff5722';
      case 'Refund':
        return '#ff9800';
      default:
        return '#666';
    }
  };

  return (
    <ScrollView style={styles.container}>
      <View style={styles.balanceCard}>
        <Text style={styles.balanceLabel}>Available Balance</Text>
        <Text style={styles.balance}>${balance.toFixed(2)}</Text>
        <Text style={styles.currency}>USD</Text>
      </View>

      <View style={styles.topUpCard}>
        <Text style={styles.sectionTitle}>Top Up Wallet</Text>
        <View style={styles.topUpContainer}>
          <TextInput
            style={styles.input}
            placeholder="Enter amount"
            keyboardType="decimal-pad"
            value={topUpAmount}
            onChangeText={setTopUpAmount}
          />
          <TouchableOpacity style={styles.topUpButton} onPress={handleTopUp}>
            <Text style={styles.topUpButtonText}>Add Funds</Text>
          </TouchableOpacity>
        </View>
        <View style={styles.quickAmounts}>
          {[10, 25, 50, 100].map(amount => (
            <TouchableOpacity
              key={amount}
              style={styles.quickButton}
              onPress={() => setTopUpAmount(amount.toString())}
            >
              <Text style={styles.quickButtonText}>${amount}</Text>
            </TouchableOpacity>
          ))}
        </View>
      </View>

      <View style={styles.transactionsCard}>
        <Text style={styles.sectionTitle}>Transaction History</Text>
        {transactions.length === 0 ? (
          <Text style={styles.noTransactions}>No transactions yet</Text>
        ) : (
          transactions.map((transaction) => (
            <View key={transaction.id} style={styles.transactionItem}>
              <View style={styles.transactionLeft}>
                <Text style={styles.transactionType}>{transaction.type}</Text>
                <Text style={styles.transactionDate}>
                  {new Date(transaction.transactionDate).toLocaleDateString()}
                </Text>
                {transaction.description && (
                  <Text style={styles.transactionDescription}>{transaction.description}</Text>
                )}
              </View>
              <Text
                style={[
                  styles.transactionAmount,
                  { color: getTransactionColor(transaction.type) }
                ]}
              >
                {transaction.amount >= 0 ? '+' : ''}${Math.abs(transaction.amount).toFixed(2)}
              </Text>
            </View>
          ))
        )}
      </View>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f5f5f5',
  },
  balanceCard: {
    backgroundColor: '#667eea',
    margin: 15,
    padding: 30,
    borderRadius: 15,
    alignItems: 'center',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 3,
  },
  balanceLabel: {
    color: 'rgba(255,255,255,0.8)',
    fontSize: 14,
    marginBottom: 10,
  },
  balance: {
    color: 'white',
    fontSize: 48,
    fontWeight: 'bold',
  },
  currency: {
    color: 'rgba(255,255,255,0.8)',
    fontSize: 16,
    marginTop: 5,
  },
  topUpCard: {
    backgroundColor: 'white',
    margin: 15,
    marginTop: 0,
    padding: 20,
    borderRadius: 15,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 2,
  },
  sectionTitle: {
    fontSize: 18,
    fontWeight: 'bold',
    marginBottom: 15,
  },
  topUpContainer: {
    flexDirection: 'row',
    gap: 10,
    marginBottom: 15,
  },
  input: {
    flex: 1,
    borderWidth: 1,
    borderColor: '#ddd',
    borderRadius: 10,
    padding: 15,
    fontSize: 16,
  },
  topUpButton: {
    backgroundColor: '#667eea',
    paddingHorizontal: 20,
    borderRadius: 10,
    justifyContent: 'center',
  },
  topUpButtonText: {
    color: 'white',
    fontWeight: 'bold',
  },
  quickAmounts: {
    flexDirection: 'row',
    gap: 10,
  },
  quickButton: {
    flex: 1,
    backgroundColor: '#f0f0f0',
    padding: 12,
    borderRadius: 8,
    alignItems: 'center',
  },
  quickButtonText: {
    fontSize: 16,
    fontWeight: '600',
    color: '#333',
  },
  transactionsCard: {
    backgroundColor: 'white',
    margin: 15,
    marginTop: 0,
    padding: 20,
    borderRadius: 15,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 2,
  },
  noTransactions: {
    textAlign: 'center',
    color: '#999',
    padding: 20,
  },
  transactionItem: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    paddingVertical: 15,
    borderBottomWidth: 1,
    borderBottomColor: '#f0f0f0',
  },
  transactionLeft: {
    flex: 1,
  },
  transactionType: {
    fontSize: 16,
    fontWeight: '600',
    marginBottom: 4,
  },
  transactionDate: {
    fontSize: 12,
    color: '#999',
  },
  transactionDescription: {
    fontSize: 12,
    color: '#666',
    marginTop: 2,
  },
  transactionAmount: {
    fontSize: 18,
    fontWeight: 'bold',
  },
});

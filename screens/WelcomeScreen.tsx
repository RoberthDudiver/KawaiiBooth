import React from 'react';
import { View, Text, TouchableOpacity, StyleSheet } from 'react-native';
import { ScreenProps } from '../types/navigation';

const WelcomeScreen: React.FC<ScreenProps<'Welcome'>> = ({ navigation }) => {
  return (
    <View style={styles.container}>
      <Text style={styles.title}>✨ KawaiiBooth ✨</Text>
      <TouchableOpacity style={styles.button} onPress={() => navigation.navigate('TemplateSelector')}>
        <Text style={styles.buttonText}>¡Comenzar!</Text>
      </TouchableOpacity>
    </View>
  );
};
const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#fff0f5', justifyContent: 'center', alignItems: 'center' },
  title: { fontSize: 32, color: '#ff69b4', fontWeight: 'bold', marginBottom: 40 },
  button: { backgroundColor: '#ffb6c1', padding: 15, borderRadius: 30 },
  buttonText: { color: '#fff', fontSize: 18 },
});
export default WelcomeScreen;

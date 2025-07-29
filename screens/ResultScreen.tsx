
import React from 'react';
import { View, Text, StyleSheet, TouchableOpacity } from 'react-native';

export default function ResultScreen({ navigation }) {
  return (
    <View style={styles.container}>
      <Text style={styles.title}>🎉 ¡Tus fotos están listas!</Text>
      <TouchableOpacity style={styles.button} onPress={() => navigation.navigate('Welcome')}>
        <Text style={styles.buttonText}>Repetir</Text>
      </TouchableOpacity>
    </View>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, justifyContent: 'center', alignItems: 'center', backgroundColor: '#fff0f5' },
  title: { fontSize: 24, marginBottom: 30, color: '#d63384' },
  button: { backgroundColor: '#ffb6c1', padding: 15, borderRadius: 20 },
  buttonText: { color: '#fff', fontSize: 16 },
});

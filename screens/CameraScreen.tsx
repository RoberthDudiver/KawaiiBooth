import React, { useEffect, useState } from 'react';
import { View, Text, StyleSheet } from 'react-native';
import { ScreenProps } from '../types/navigation';

export default function CameraScreen({ navigation }: ScreenProps<'Camera'>) {
  const [counter, setCounter] = useState(3);

  useEffect(() => {
    if (counter > 0) {
      const timer = setTimeout(() => setCounter(counter - 1), 1000);
      return () => clearTimeout(timer);
    } else {
      navigation.navigate('Result');
    }
  }, [counter]);

  return (
    <View style={styles.container}>
      <Text style={styles.counter}>{counter > 0 ? counter : "¡Foto lista!"}</Text>
    </View>
  );
}


const styles = StyleSheet.create({
  container: { flex: 1, justifyContent: 'center', alignItems: 'center', backgroundColor: '#ffe' },
  counter: { fontSize: 48, color: '#ff69b4', fontWeight: 'bold' },
});

import React from 'react';
import { View, Text, TouchableOpacity, Image, StyleSheet } from 'react-native';
import { ScreenProps } from '../types/navigation';

export default function TemplateSelectorScreen({ navigation }: ScreenProps<'TemplateSelector'>) {
  return (
    <View style={styles.container}>
      <Text style={styles.title}>Elige tu plantilla</Text>
      <TouchableOpacity onPress={() => navigation.navigate('Camera')}>
        <Image source={require('../assets/template1.png')} style={styles.template} />
      </TouchableOpacity>
    </View>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, alignItems: 'center', justifyContent: 'center', backgroundColor: '#fffafc' },
  title: { fontSize: 24, marginBottom: 20 },
  template: { width: 200, height: 300, resizeMode: 'contain', borderRadius: 10 },
});

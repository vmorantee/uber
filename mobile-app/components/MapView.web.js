import { Platform } from 'react-native';

let MapView;
let Marker;

if (Platform.OS === 'web') {
  MapView = ({ children, style, ...props }) => {
    return null;
  };
  
  Marker = ({ children, ...props }) => {
    return null;
  };
  
  MapView.Marker = Marker;
} else {
  const Maps = require('react-native-maps');
  MapView = Maps.default;
  Marker = Maps.Marker;
}

export default MapView;
export { Marker };

# 📱 Instrukcja Uruchomienia Aplikacji Mobilnej

## ✅ Wymagania

1. **Node.js 18+** - zainstalowany
2. **Telefon Android/iOS** z zainstalowaną aplikacją **Expo Go**
   - Android: [Google Play - Expo Go](https://play.google.com/store/apps/details?id=host.exp.exponent)
   - iOS: [App Store - Expo Go](https://apps.apple.com/app/expo-go/id982107779)

---

## 🚀 Kroki Uruchomienia

### KROK 1: Przejdź do folderu aplikacji

```powershell
cd C:\Users\msmar\Prajetoo\mobile-app
```

### KROK 2: Zainstaluj zależności (jeśli jeszcze nie zainstalowane)

```cmd
npm install
```

Instalacja zajmie ~2-3 minuty.

### KROK 3: Uruchom Expo Dev Server

**Przez CMD (zalecane jeśli PowerShell ma problem z wykonywaniem skryptów):**

```cmd
cmd /c "cd C:\Users\msmar\Prajetoo\mobile-app && npx expo start"
```

**LUB przez PowerShell (jeśli włączone skrypty):**

```powershell
npx expo start
```

### KROK 4: Zeskanuj QR Code

Po uruchomieniu zobaczysz:
```
› Metro waiting on exp://192.168.1.13:8081
› Scan the QR code above with Expo Go (Android) or the Camera app (iOS)

› Press a │ open Android
› Press i │ open iOS simulator
› Press w │ open web

› Press r │ reload app
› Press m │ toggle menu
```

**Na telefonie:**
1. Otwórz **Expo Go**
2. Zeskanuj **QR code** z terminala
3. Aplikacja załaduje się automatycznie

---

## 📋 Konfiguracja (WAŻNE!)

### ✅ Adres IP został już ustawiony na: `192.168.1.13`

Sprawdź pliki:
- `mobile-app/services/api.js` → `API_BASE_URL = 'http://192.168.1.13:5000/api'`
- `mobile-app/services/signalr.js` → `HUB_URL = 'http://192.168.1.13:5000/hubs/ride'`

**Jeśli Twój adres IP się zmieni**, uruchom:
```powershell
ipconfig | Select-String "IPv4"
```
I zaktualizuj oba pliki.

---

## 👥 Testowe Konta do Logowania

### Pasażerowie:
| Email | Hasło |
|-------|-------|
| jan.kowalski@example.com | Test123! |
| anna.nowak@example.com | Test123! |

### Kierowcy (Zatwierdzeni):
| Email | Hasło |
|-------|-------|
| piotr.wisniewski@example.com | Test123! |
| maria.lewandowska@example.com | Test123! |

---

## 🎯 Scenariusze Testowe

### Scenariusz 1: Pasażer zamawia przejazd

1. **Zaloguj się** jako pasażer (jan.kowalski@example.com / Test123!)
2. Zobaczysz:
   - Banery promocyjne na górze
   - Formularz zamówienia przejazdu
3. **Podaj lokalizacje:**
   - Start Lat: `52.2297` Lng: `21.0122` (Warszawa)
   - End Lat: `52.4064` Lng: `16.9252` (Poznań)
4. **Kliknij "Estimate Price"** - zobaczysz szacowaną cenę (~280 km)
5. **Kliknij "Request Ride"** - utworzy się przejazd

### Scenariusz 2: Kierowca akceptuje przejazd

1. **Wyloguj się** i zaloguj jako kierowca (piotr.wisniewski@example.com / Test123!)
2. **Przełącz się w tryb Driver** (przycisk u góry ekranu)
3. **Kliknij "Go Online"**
4. Zobaczysz:
   - Status: "🟢 Online"
   - Lista dostępnych przejazdów
5. **Kliknij "Accept Ride"** przy wybranym przejeździe
6. **Kliknij "Start Ride"** - rozpocznie się przejazd
7. **Kliknij "Complete Ride"** - zakończy przejazd i automatyczna płatność

### Scenariusz 3: Real-time Tracking

1. **Otwórz 2 telefony** (lub przeglądarkę + telefon)
2. Telefon 1: Pasażer zamawia przejazd
3. Telefon 2: Kierowca akceptuje i startuje
4. Telefon 1: Pasażer widzi na żywo:
   - Powiadomienie "Ride Accepted"
   - Dane kierowcy (imię, telefon, auto)
   - **Live tracking lokalizacji** kierowcy (aktualizacja co 5s)
   - Status przejazdu (ACCEPTED → STARTED → COMPLETED)

---

## 🐛 Rozwiązywanie Problemów

### Problem: "Cannot connect to server"
**Rozwiązanie:**
1. Sprawdź czy backend działa: `docker-compose ps`
2. Upewnij się że telefon i komputer są w **tej samej sieci WiFi**
3. Sprawdź firewall - port 5000 musi być otwarty
4. Zweryfikuj IP w `services/api.js` i `services/signalr.js`

### Problem: "Network request failed"
**Rozwiązanie:**
```powershell
# Sprawdź czy API odpowiada
curl http://192.168.1.13:5000/swagger
```

### Problem: PowerShell blokuje npm
**Rozwiązanie:**
Użyj CMD zamiast PowerShell:
```cmd
cmd /c "cd C:\Users\msmar\Prajetoo\mobile-app && npx expo start"
```

**LUB włącz wykonywanie skryptów (jako Administrator):**
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### Problem: "expo: command not found"
**Rozwiązanie:**
Użyj `npx expo` zamiast `expo`:
```cmd
npx expo start
```

---

## 📱 Ekrany Aplikacji

1. **LoginScreen** - Logowanie / Rejestracja
2. **RideFlowScreen** - Tryb Pasażera (zamów przejazd, tracking)
3. **DriverScreen** - Tryb Kierowcy (online/offline, akceptuj przejazdy)
4. **WalletScreen** - Portfel (saldo, historia transakcji, wpłaty)
5. **ProfileScreen** - Profil użytkownika (dane, przełącznik Passenger/Driver)

---

## 🎨 Funkcje Aplikacji

### ✅ Zaimplementowane:

- ✅ **Logowanie i Rejestracja** (JWT)
- ✅ **Dual Mode**: Przełączanie Passenger ↔ Driver
- ✅ **Estymacja przejazdu** (dystans + cena)
- ✅ **Real-time tracking** (SignalR - lokalizacja co 5s)
- ✅ **Banery promocyjne** (z backendu)
- ✅ **Portfel** (saldo, wpłaty, historia)
- ✅ **Powiadomienia real-time** (RideAccepted, RideStarted, RideCompleted)
- ✅ **Mapa** (MapView z Expo)
- ✅ **AsyncStorage** dla tokenu JWT
- ✅ **Context API** dla stanu globalnego

---

## 🔧 Skróty Klawiszowe w Expo

Po uruchomieniu `npx expo start`:

| Klawisz | Akcja |
|---------|-------|
| `a` | Otwórz na emulatorze Android |
| `i` | Otwórz na symulatorze iOS |
| `w` | Otwórz w przeglądarce web |
| `r` | Przeładuj aplikację |
| `m` | Toggle menu |
| `c` | Wyczyść cache i restartuj |

---

## 📚 Dokumentacja

- **Expo Docs**: https://docs.expo.dev/
- **React Native**: https://reactnative.dev/
- **SignalR Client**: https://www.npmjs.com/package/@microsoft/signalr

---

**Aplikacja została skonfigurowana i jest gotowa do uruchomienia! 🚀**

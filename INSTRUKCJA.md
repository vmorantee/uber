# 🚗 Ride-Sharing System - Kompletna Instrukcja

## ✅ SYSTEM JEST JUŻ URUCHOMIONY!

Wszystkie serwisy działają pod następującymi adresami:

### 🌐 Interfejsy Web

| Serwis | Adres | Opis |
|--------|-------|------|
| **Backend API (Swagger)** | http://localhost:5000/swagger | Dokumentacja API i testowanie endpointów |
| **Admin Panel** | http://localhost:8080 | Zarządzanie użytkownikami, sporami, banerami |
| **Landing Page** | http://localhost:5001 | Strona rekrutacji kierowców |
| **SQL Server** | localhost:1433 | Baza danych (sa / RideSharing@123) |

---

## 👥 Testowe Konta Użytkowników

### Pasażerowie

| Email | Hasło | Saldo Portfela |
|-------|-------|----------------|
| jan.kowalski@example.com | Test123! | 150.00 PLN |
| anna.nowak@example.com | Test123! | 200.00 PLN |

### Kierowcy (Zatwierdzeni - APPROVED)

| Email | Hasło | Saldo Portfela | Status |
|-------|-------|----------------|--------|
| piotr.wisniewski@example.com | Test123! | 500.00 PLN | ✅ APPROVED |
| maria.lewandowska@example.com | Test123! | 300.00 PLN | ✅ APPROVED |

### Kierowcy (Oczekujący - PENDING)

| Email | Hasło | Status |
|-------|-------|--------|
| tomasz.kaminski@example.com | Test123! | ⏳ PENDING |

---

## 🔄 Jak Zatrzymać i Uruchomić Ponownie?

### Zatrzymanie wszystkich kontenerów:
```powershell
docker-compose down
```

### Ponowne uruchomienie:
```powershell
docker-compose up
```

### Przebudowanie i uruchomienie (jeśli zmieniony kod):
```powershell
docker-compose up --build
```

### **WAŻNE**: Pierwsza Inicjalizacja Bazy Danych

Przy pierwszym uruchomieniu lub po wykonaniu `docker-compose down -v`, musisz zainicjalizować bazę danych:

**OPCJA 1 - Skrypt Automatyczny (Zalecany):**
```powershell
# Po uruchomieniu docker-compose up, w NOWYM oknie terminala:
.\init-database.bat
```

**OPCJA 2 - Ręcznie przez PowerShell:**
```powershell
# Poczekaj 15-20 sekund po docker-compose up, następnie:
docker run --rm --network prajetoo_ridesharing-network -v "${PWD}/database:/scripts" mcr.microsoft.com/mssql-tools /opt/mssql-tools/bin/sqlcmd -S ridesharing-sqlserver -U sa -P 'RideSharing@2026!' -i /scripts/init.sql

docker run --rm --network prajetoo_ridesharing-network -v "${PWD}/database:/scripts" mcr.microsoft.com/mssql-tools /opt/mssql-tools/bin/sqlcmd -S ridesharing-sqlserver -U sa -P 'RideSharing@2026!' -i /scripts/seed.sql
```

**Wynik powinieneś zobaczyć:**
```
Changed database context to 'RideSharingDB'.
(6 rows affected)  -- Users
(6 rows affected)  -- Wallets
(7 rows affected)  -- WalletTransactions
(4 rows affected)  -- Rides
(1 rows affected)  -- Disputes
(2 rows affected)  -- DriverLocations
(2 rows affected)  -- Banners
```

### Usunięcie wszystkiego (włącznie z danymi):
```powershell
docker-compose down -v
# Po ponownym uruchomieniu pamiętaj o init-database.bat!
```

---

## 📱 Aplikacja Mobilna (React Native)

### 1. Instalacja zależności:

```powershell
cd mobile-app
npm install
```

### 2. **WAŻNE**: Zmiana adresu API

Edytuj plik `mobile-app/services/api.js`:

```javascript
const API_BASE_URL = 'http://192.168.1.XXX:5000';
```

Zamień `192.168.1.XXX` na **adres IP twojego komputera** (nie używaj `localhost`!)

**Jak znaleźć swój adres IP?**
```powershell
ipconfig
```
Szukaj `IPv4 Address` w sekcji `Wireless LAN adapter Wi-Fi` lub `Ethernet adapter`

### 3. Uruchomienie aplikacji:

```powershell
npx expo start
```

### 4. Testowanie:

- **Android**: Zainstaluj **Expo Go** z Google Play i zeskanuj QR code
- **iOS**: Zainstaluj **Expo Go** z App Store i zeskanuj QR code
- **Web**: Naciśnij klawisz `w` w terminalu

---

## 🎯 Typowy Scenariusz Użycia

### SCENARIUSZ 1: Pasażer zamawia przejazd

1. **Otwórz aplikację mobilną** i zaloguj się jako pasażer:
   - Email: `jan.kowalski@example.com`
   - Hasło: `Test123!`

2. **Na ekranie głównym zobaczysz**:
   - Banery promocyjne
   - Formularz zamówienia przejazdu

3. **Podaj lokalizacje**:
   - Start: Warszawa (52.2297, 21.0122)
   - Koniec: Poznań (52.4064, 16.9252)

4. **Kliknij "Estimate Price"** - otrzymasz szacowaną cenę

5. **Kliknij "Request Ride"** - przejazd zostanie utworzony

### SCENARIUSZ 2: Kierowca akceptuje przejazd

1. **W drugiej instancji aplikacji** zaloguj się jako kierowca:
   - Email: `piotr.wisniewski@example.com`
   - Hasło: `Test123!`

2. **Przełącz się w tryb Driver** (przycisk u góry)

3. **Kliknij "Go Online"**:
   - Aplikacja zacznie wysyłać twoją lokalizację co 5 sekund
   - Wyświetli się lista dostępnych przejazdów

4. **Kliknij "Accept Ride"** przy wybranym przejeździe

5. **Pasażer otrzyma powiadomienie** (SignalR) z danymi kierowcy

6. **Kierowca może**:
   - Kliknąć "Start Ride" - rozpoczęcie przejazdu
   - Kliknąć "Complete Ride" - zakończenie i automatyczna płatność

### SCENARIUSZ 3: Admin zarządza systemem

1. **Otwórz Admin Panel**: http://localhost:8080

2. **Zatwierdzanie kierowców**:
   - Kliknij "Users" w menu
   - Znajdź kierowcę ze statusem PENDING
   - Zmień status na APPROVED
   - Zapisz

3. **Rozwiązywanie sporów**:
   - Kliknij "Disputes" w menu
   - Otwórz spór
   - Wybierz rozwiązanie:
     - REFUND_PASSENGER - zwrot pieniędzy pasażerowi
     - COMPENSATE_DRIVER - kompensata dla kierowcy
   - System automatycznie zaktualizuje portfele

4. **Zarządzanie banerami**:
   - Kliknij "Banners" w menu
   - Dodaj nowy banner (upload obrazu)
   - Banner pojawi się w aplikacji mobilnej

---

## 🛠️ Testowanie API przez Swagger

### 1. Otwórz Swagger UI: http://localhost:5000/swagger

### 2. Test logowania:

1. Rozwiń endpoint `POST /api/auth/login`
2. Kliknij "Try it out"
3. Wpisz:
```json
{
  "email": "jan.kowalski@example.com",
  "password": "Test123!"
}
```
4. Kliknij "Execute"
5. **Skopiuj `token` z odpowiedzi**

### 3. Autoryzacja:

1. Kliknij przycisk **"Authorize"** u góry strony
2. Wpisz: `Bearer TWOJ_TOKEN`
3. Kliknij "Authorize"

### 4. Test estymacji przejazdu:

1. Rozwiń endpoint `POST /api/ride/estimate`
2. Kliknij "Try it out"
3. Wpisz:
```json
{
  "startLocationLat": 52.2297,
  "startLocationLng": 21.0122,
  "endLocationLat": 52.4064,
  "endLocationLng": 16.9252
}
```
4. Kliknij "Execute"
5. Zobaczysz szacowaną cenę i czas

---

## 📊 Monitoring i Debugging

### Sprawdzenie statusu kontenerów:
```powershell
docker-compose ps
```

### Logi wszystkich serwisów:
```powershell
docker-compose logs -f
```

### Logi konkretnego serwisu:
```powershell
docker-compose logs -f dotnet-api
docker-compose logs -f springboot-admin
docker-compose logs -f aspnet-landing
docker-compose logs -f sqlserver
```

### Restart konkretnego serwisu:
```powershell
docker-compose restart dotnet-api
```

---

## 🗄️ Struktura Bazy Danych

System automatycznie tworzy bazę `RideSharingDB` z tabelami:

| Tabela | Opis | Kluczowe Kolumny |
|--------|------|------------------|
| **Users** | Użytkownicy systemu | Email, Role (PASSENGER/DRIVER), ApprovalStatus |
| **Wallets** | Portfele użytkowników | UserId, Balance |
| **WalletTransactions** | Historia transakcji | WalletId, Amount, Type (DEPOSIT/WITHDRAWAL) |
| **Rides** | Przejazdy | PassengerId, DriverId, Status, Price |
| **DriverLocations** | Real-time lokalizacje | DriverId, Latitude, Longitude |
| **Disputes** | Spory | RideId, Status (OPEN/RESOLVED) |
| **Banners** | Banery promocyjne | ImageUrl, IsActive |

---

## 🔧 Kluczowe Endpointy API

### Autoryzacja
- `POST /api/auth/register` - Rejestracja użytkownika
- `POST /api/auth/login` - Logowanie
- `GET /api/auth/me` - Dane zalogowanego użytkownika
- `PUT /api/auth/apply-driver` - Aplikacja na kierowcę

### Przejazdy
- `POST /api/ride/estimate` - Estymacja ceny
- `POST /api/ride/create` - Utworzenie przejazdu
- `POST /api/ride/accept` - Akceptacja przez kierowcę
- `POST /api/ride/start` - Start przejazdu
- `POST /api/ride/complete` - Zakończenie przejazdu
- `POST /api/ride/location` - Aktualizacja lokalizacji kierowcy
- `GET /api/ride/available` - Dostępne przejazdy dla kierowcy
- `GET /api/ride/{id}` - Szczegóły przejazdu

### Portfel
- `GET /api/wallet` - Saldo portfela
- `POST /api/wallet/deposit` - Wpłata
- `GET /api/wallet/transactions` - Historia transakcji

### SignalR Hub (Real-time)
- **Endpoint**: `http://localhost:5000/ridehub`
- **Events**:
  - `RideAccepted` - Przejazd zaakceptowany
  - `RideStarted` - Przejazd rozpoczęty
  - `RideCompleted` - Przejazd zakończony
  - `DriverLocationUpdated` - Aktualizacja lokalizacji kierowcy

---

## 🎨 Technologie i Wzorce

### Backend (.NET Core 8)
- ✅ Repository Pattern
- ✅ Dependency Injection
- ✅ JWT Authentication
- ✅ Entity Framework Core
- ✅ SignalR dla real-time
- ✅ Azure Service Bus integration
- ✅ Swagger/OpenAPI

### Admin Panel (Spring Boot 3)
- ✅ MVC Pattern
- ✅ Thymeleaf Templates
- ✅ JPA/Hibernate
- ✅ HikariCP Connection Pool
- ✅ Azure Blob Storage
- ✅ Bootstrap 5

### Landing Page (ASP.NET Core MVC)
- ✅ Razor Views
- ✅ Model Binding
- ✅ Tag Helpers
- ✅ Responsive Design

### Mobile App (React Native)
- ✅ Functional Components
- ✅ React Hooks
- ✅ Context API
- ✅ SignalR Client
- ✅ React Navigation
- ✅ MapView (Expo)

---

## ⚙️ Zmienne Środowiskowe (Opcjonalne)

### Azure Service Bus (Backend API)
```
AZURE_SERVICEBUS_CONNECTION=Endpoint=sb://...
```

### Azure Blob Storage (Admin Panel)
```
AZURE_STORAGE_CONNECTION=DefaultEndpointsProtocol=https;...
```

Jeśli nie są skonfigurowane, system działa w **trybie symulacji**.

---

## 🚨 Troubleshooting

### Problem: Kontenery nie startują
**Rozwiązanie**: Sprawdź logi:
```powershell
docker-compose logs
```

### Problem: Aplikacja mobilna nie łączy się z API
**Rozwiązanie**: 
1. Sprawdź czy używasz adresu IP (nie localhost)
2. Upewnij się, że firewall nie blokuje portu 5000
3. Sprawdź czy telefon jest w tej samej sieci WiFi

### Problem: Baza danych nie została zainicjalizowana
**Rozwiązanie**: 
```powershell
docker-compose down -v
docker-compose up --build
```

### Problem: Port już zajęty (np. 5000, 8080, 5001)
**Rozwiązanie**: Zmień port w `docker-compose.yml`:
```yaml
ports:
  - "5050:5000"  # Nowy port : stary port
```

---

## 📝 Clean Code Principles

Projekt został zbudowany zgodnie z zasadami Clean Code:
- ✅ Brak komentarzy w kodzie (kod mówi sam za siebie)
- ✅ Nazwy zmiennych i metod są self-explanatory
- ✅ Single Responsibility Principle
- ✅ Dependency Injection
- ✅ Repository Pattern
- ✅ Separation of Concerns

---

## 🎯 Następne Kroki (Opcjonalne)

### Produkcja:
1. Skonfiguruj Azure Service Bus
2. Skonfiguruj Azure Blob Storage
3. Dodaj SSL/TLS certyfikaty
4. Skonfiguruj CI/CD pipeline
5. Dodaj Application Insights monitoring
6. Dodaj testy jednostkowe i integracyjne

### Development:
1. Dodaj więcej typów przejazdów (Premium, Shared)
2. Dodaj system ocen kierowców
3. Dodaj mapę w czasie rzeczywistym
4. Dodaj chat między pasażerem a kierowcą
5. Dodaj powiadomienia push

---

## 📞 Kontakt

Projekt stworzony jako kompletne demo systemu Ride-Sharing.

---

**GOTOWE! System działa i jest w pełni funkcjonalny! 🎉**

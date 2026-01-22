# 🚀 Quick Start - Ride-Sharing System

## Szybkie Uruchomienie w 3 Krokach

### KROK 1: Uruchom Docker Compose
```powershell
docker-compose up --build
```
Czekaj aż zobaczysz:
- ✅ `ridesharing-sqlserver   Up X seconds (healthy)`
- ✅ `ridesharing-api         Up X seconds`
- ✅ `ridesharing-admin       Up X seconds`
- ✅ `ridesharing-landing     Up X seconds`

### KROK 2: Zainicjalizuj Bazę Danych (NOWE okno terminala)
```powershell
.\init-database.bat
```

**LUB ręcznie:**
```powershell
docker run --rm --network prajetoo_ridesharing-network -v "${PWD}/database:/scripts" mcr.microsoft.com/mssql-tools /opt/mssql-tools/bin/sqlcmd -S ridesharing-sqlserver -U sa -P 'RideSharing@2026!' -i /scripts/init.sql

docker run --rm --network prajetoo_ridesharing-network -v "${PWD}/database:/scripts" mcr.microsoft.com/mssql-tools /opt/mssql-tools/bin/sqlcmd -S ridesharing-sqlserver -U sa -P 'RideSharing@2026!' -i /scripts/seed.sql
```

### KROK 3: Otwórz w Przeglądarce

- **Swagger API**: http://localhost:5000/swagger
- **Admin Panel**: http://localhost:8080
- **Landing Page**: http://localhost:5001

---

## ✅ Testowe Konta

| Email | Hasło | Rola |
|-------|-------|------|
| jan.kowalski@example.com | Test123! | Passenger |
| anna.nowak@example.com | Test123! | Passenger |
| piotr.wisniewski@example.com | Test123! | Driver (APPROVED) |
| maria.lewandowska@example.com | Test123! | Driver (APPROVED) |
| tomasz.kaminski@example.com | Test123! | Driver (PENDING) |

---

## 🧪 Test API przez Swagger

1. Otwórz http://localhost:5000/swagger
2. Kliknij `POST /api/auth/login`
3. Kliknij "Try it out"
4. Wpisz:
```json
{
  "email": "jan.kowalski@example.com",
  "password": "Test123!"
}
```
5. Kliknij "Execute"
6. Skopiuj `token` z odpowiedzi
7. Kliknij **"Authorize"** u góry
8. Wpisz: `Bearer TWOJ_TOKEN`
9. Teraz możesz testować wszystkie endpointy!

---

## 📱 Mobile App (Opcjonalnie)

```powershell
cd mobile-app
npm install
```

**ZMIEŃ IP w `mobile-app/services/api.js`:**
```javascript
const API_BASE_URL = 'http://TWOJ_IP:5000';  // Nie localhost!
```

Znajdź swój IP:
```powershell
ipconfig
```

Uruchom:
```powershell
npx expo start
```

---

## 🛑 Zatrzymanie

```powershell
docker-compose down
```

---

## 📚 Pełna Dokumentacja

Zobacz plik `INSTRUKCJA.md` dla kompletnej dokumentacji!

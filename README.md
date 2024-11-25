# Kartverket

Velkommen til **Kartverket-prosjektet**! Dette systemet gir effektiv håndtering av geografiske data, og lar brukere registrere og se kartendringer enkelt.

---

## 📄 Dokumentasjon

For mer teknisk dokumentasjon, besøk [Wiki](https://github.com/Gruppe12/Kartverket/wiki).

---

## ✨ Funksjoner

- **Rollehåndtering**:
  - **Saksbehandler**: Behandle og gjennomgå alle innsendte kartendringer.
  - **Bruker**: Registrere og spore egne kartendringer.
- **Interaktive kart**: Tegn og rediger polygoner med **Leaflet**, støtter GeoJSON-format.
- **API-integrasjon**: Hent kommunedata via **Kartverkets API-er**.
- **Sikker tilgang**: Rollebasert tilgangsstyring via **ASP.NET Core Identity**.

---

## 🚀 Installasjon

### Forutsetninger

Sørg for at følgende er installert:
- [Docker](https://www.docker.com/)
- [Visual Studio](https://visualstudio.microsoft.com/)
- [.NET SDK](https://dotnet.microsoft.com/download)

### Kom i gang

1. **Klone repoet**:
    ```bash
    git clone https://github.com/Gruppe12/Kartverket.git
    cd Kartverket
    ```

2. **Start Docker-containere**:
    - Åpne prosjektet i Visual Studio.
    - Velg **Docker Compose** som startprosjekt.
    - Kjør applikasjonen for å starte containerne.

3. **Migrer databasen**:
    Oppdater MariaDB-databaseskjemaet med:
    ```bash
    dotnet ef database update
    ```

---

## 🖥️ Teknologier i bruk

- **MariaDB**: Relasjonsdatabase, hostet i Docker.
- **ASP.NET Core**: Backend-rammeverk for API-er og logikk.
- **Leaflet**: JavaScript-bibliotek for interaktive kart.
- **Docker**: Containerbasert løsning for distribusjon.

---

## 🧑‍💻 Demobruker

For å utforske applikasjonen, bruk følgende innloggingsinformasjon:

- **Brukernavn**: `Admin`
- **Passord**: `Admin123!`


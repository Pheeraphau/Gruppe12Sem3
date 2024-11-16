# Prosjekt: Kartverket

## Applikasjonens Arkitektur
Applikasjonen er laget med Model-View-Controller arkitektur:
- Models: Håndterer data.
- Views: Fremstiller dataen til brukeren.
- Controllers: Er et mellomledd for models og views og oppdaterer de. 
- Database: MariaDB database som lagrer data om brukere og karteendringer. 

## Drift
### Forutsetninger
For å kjøre applikasjonen trenger du:
- Docker Desktop
- Visual Studio

### Oppsett
1. Kopier URLen til repositoryen.
2. I Visual Studio trykk på "CLone a repository".
3. Putt inn lenken og trykk på "Clone".
4. I Visual Studio trykk på "Build Solution" som finnes i build menyen.
5. Kjør med Docker Compose

## Funksjonaliteter
- Registrering av ny bruker
- Bruker kan logge inn
- Bruker kan legge til, endre og slette innmeldinger i databasen
- Kartvisning med mulighet for karteendringer

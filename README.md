Prosjekt: Kartverket

Applikasjonens Arkitektur

Applikasjonen er laget med Model-View-Controller arkitektur:

Models: Håndterer data.
Views: Fremstiller dataen til brukeren.
Controllers: Er et mellomledd for models og views og oppdaterer de.
Database: MariaDB database som lagrer data om brukere og karteendringer.
Drift

Forutsetninger

For å kjøre applikasjonen trenger du:

Docker Desktop
Visual Studio
Oppsett

Kopier URLen til repositoryen.
I Visual Studio trykk på "CLone a repository".
Putt inn lenken og trykk på "Clone".
I Visual Studio trykk på "Build Solution" som finnes i build menyen.
Kjør med Docker Compose
Funksjonaliteter

Kartvisning med mulighet for karteendringer

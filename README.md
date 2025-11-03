# Automat

Kratak opis
- Desktop aplikacija koja simulira automat za prodaju (vending machine) sa mrežom od 3x4 slotova.
- Korisnik može ubaciti novac, odabrati artikal, kupiti ga i uzeti kusur.
- Postoji administratorski deo za dodavanje i uređivanje artikala i kase.

Glavne funkcionalnosti
- Prikaz kredita i kupovina artikala klikom na slot ili ručnim unosom pozicije (red/kolona).
- `Ubaci novac` i `Uzmi kusur` akcije.
- Admin panel (pristup preko `Admin` dugmeta) sa:
  - Dodavanjem/izmenom artikala (naziv, veličina, rok, cena, količina).
  - Ažuriranjem stanja kase.
- Ekrani/prozori: `MainWindow`, `AdminWindow`, `AddArtikalWindow`, `AddMoney`, `Login`.

Tehnologije
- .NET 8
- WPF (XAML) i C#

Struktura (ključne datoteke)
- UI: `MainWindow.xaml`, `AdminWindow.xaml`, `AddArtikalWindow.xaml`, `AddMoney.xaml`, `Login.xaml` (+ pripadajući `.xaml.cs`).
- Modeli i logika: `Artikal.cs`, `Kasa.cs`.

Pokretanje
1. Requirements: Windows, .NET 8 SDK, Visual Studio 2022 (sa .NET desktop development workload-om).
2. Otvoriti rešenje/projekat u Visual Studio-u i pokrenuti `Vending Machine` projekat (F5).
   - Alternativno iz terminala (na Windows-u): u folderu projekta `dotnet build` pa `dotnet run`.

Kompatibilnost sa .NET 7
- Projekat po difoltu cilja .NET 8. Ako je potrebno, može raditi i na .NET 7, ali neke funkcionalnosti dostupne u .NET 8 možda neće biti prisutne u .NET 7.
- Promena ciljne verzije (TFM) se vrši u `.csproj` fajlu:
  1. Otvorite `Vending Machine.csproj`.
  2. Zamenite liniju za verziju framework-a, npr.:
     - Sa: `<TargetFramework>net8.0-windows</TargetFramework>`
     - Na: `<TargetFramework>net7.0-windows</TargetFramework>`
  3. Sačuvajte fajl i ponovo pokrenite build.
- Napomena: `-windows` sufiks je potreban za WPF. Neke specifičnosti za .NET 8 možda neće biti dostupne na .NET 7.
# Authentik CSV User Importer

Ein Kommandozeilen-Tool zum Massenimport von Benutzern aus einer CSV-Datei in [Authentik](https://goauthentik.io/). Legt Benutzer an, fügt sie einer Gruppe hinzu und kann optional automatisch eine Passwort-Reset-E-Mail auslösen.

## Funktionen

- 📄 Import von Benutzern aus einer CSV-Datei
- 🔁 Automatische Konfliktauflösung bei bereits vergebenen Benutzernamen
- 👥 Automatisches Hinzufügen zu einer Authentik-Gruppe
- 📧 Optionaler Versand von Passwort-Reset-E-Mails
- 📊 Zusammenfassung erfolgreicher und fehlgeschlagener Importe am Ende
- ⚙️ Einzelne Fehler brechen den Gesamtimport nicht ab

## Download

Fertige, lauffähige Programme (keine .NET-Installation nötig) findest du auf der [Releases-Seite](../../releases):

| Betriebssystem | Datei |
|---|---|
| Windows | `AuthentikUserImporter-win-x64.zip` |
| Linux | `AuthentikUserImporter-linux-x64.zip` |
| macOS | `AuthentikUserImporter-osx-x64.zip` |

Zip-Datei entpacken und die enthaltene Programmdatei ausführen.

## Voraussetzungen

- Ein Authentik-Zugriffstoken mit Berechtigung zum Anlegen von Benutzern und Gruppenzuweisungen
- Die ID der Zielgruppe in Authentik
- Optional: Name der E-Mail-Stage, falls automatische Passwort-Reset-Mails verschickt werden sollen
- Eine CSV-Datei mit den zu importierenden Benutzern

## CSV-Format

<!-- TODO: An das tatsächliche Format anpassen -->
```csv
FirstName,LastName,Email,EmailPasswordResetLink
Max,Mustermann,max.mustermann@example.com,true
```

## Verwendung

Programm starten:

```bash
./AuthentikUserImporter
```

Das Tool fragt interaktiv nach:

1. Pfad zur CSV-Datei
2. Authentik Base-URL (z. B. `https://auth.example.com`)
3. Authentik API-Token
4. Authentik Gruppen-ID
5. Name der E-Mail-Stage (für Passwort-Reset-Mails)

Bereits bekannte Werte werden in Klammern als Vorschlag angezeigt — einfach `Enter` drücken, um den vorgeschlagenen Wert zu übernehmen.

Nach Eingabe aller Parameter wird eine Bestätigung angefordert, bevor der eigentliche Import startet.

Am Ende des Imports zeigt das Tool eine Zusammenfassung mit erfolgreich angelegten Benutzern (inkl. UUID und finalem Benutzernamen) sowie ggf. aufgetretenen Fehlern an.

## Aus dem Quellcode bauen

Voraussetzung: [.NET 8 SDK](https://dotnet.microsoft.com/download)

```bash
git clone https://github.com/<org>/<repo>.git
cd <repo>
dotnet build
dotnet run --project AuthentikUserImporter
```

Self-contained Binary selbst erstellen:

```bash
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true
```

(`linux-x64` durch `win-x64` oder `osx-x64` ersetzen, je nach Zielsystem.)

## Releases erstellen (für Maintainer)

Neue Releases werden automatisch per GitHub Actions gebaut, sobald ein Tag im Format `vX.Y.Z` gepusht wird:

```bash
git tag v1.0.0
git push origin v1.0.0
```

## Sicherheitshinweis

Das Authentik-Token wird nur lokal in der laufenden Sitzung verwendet und nicht gespeichert oder übertragen. Trotzdem sollte das Token nur mit den minimal notwendigen Berechtigungen ausgestattet sein und nach Gebrauch bei Bedarf widerrufen werden.

## Lizenz

<!-- TODO: Lizenz ergänzen -->

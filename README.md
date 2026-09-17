# Authentik User Importer

Ein CLI-Tool zum Massenimport von Benutzer:innen in [Authentik](https://goauthentik.io/) über eine CSV-Datei.

## Funktionen

- Anlegen von Benutzern in Authentik anhand einer CSV-Liste
- Automatische Vermeidung doppelter Benutzernamen (bei Konflikt wird ein alternativer Name vorgeschlagen)
- Optionale Zuweisung zu einer Gruppe
- Optionaler Versand einer E-Mail zum Setzen des Passworts
- Abschließende Zusammenfassung mit erfolgreich angelegten und fehlgeschlagenen Einträgen

## Installation

Fertige Binaries stehen unter [Releases](../../releases) zum Download bereit:

| Betriebssystem | Datei |
|---|---|
| Windows | `AuthentikUserImporter-win-x64.zip` |
| Linux | `AuthentikUserImporter-linux-x64.zip` |
| macOS | `AuthentikUserImporter-osx-x64.zip` |

Die ZIP-Datei entpacken; eine zusätzliche Installation ist nicht erforderlich.

> **Hinweis (Windows):** Da die Binaries nicht signiert sind, kann beim ersten Start eine SmartScreen-Warnung erscheinen. Über „Weitere Informationen" → „Trotzdem ausführen" fortfahren.

## Voraussetzungen

Vor dem ersten Einsatz werden folgende Informationen benötigt:

| Information | Beschreibung |
|---|---|
| Authentik-URL | Basis-URL der eigenen Authentik-Instanz, z. B. `https://auth.example.com` |
| API-Token | Token mit ausreichender Berechtigung, erstellbar unter `/if/admin/#/core/tokens` |
| Gruppen-ID | ID der Zielgruppe, sofern die Benutzer einer Gruppe zugeordnet werden sollen. Kann leer gelassen werden. |
| E-Mail-Stage | Name der Stage für den Passwort-Recovery-Versand, falls automatisch E-Mails verschickt werden sollen |

> **Hinweis:** Das API-Token ist sensibel und sollte wie ein Passwort behandelt werden.

## CSV-Format

Die Eingabedatei ist semikolon-getrennt und enthält folgende Spalten:

```csv
firstname;lastname;email;phonenumber;sms;path
Max;Mustermann;max.mustermann@example.com;+491234567890;true;users
```

| Spalte | Pflicht | Beschreibung |
|---|---|---|
| `firstname` | ja | Vorname |
| `lastname` | ja | Nachname |
| `email` | ja | E-Mail-Adresse |
| `phonenumber` | nein | Telefonnummer |
| `sms` | nein | `true`/`false` – Benachrichtigung per SMS. Standard: `false` |
| `path` | nein | Pfad, unter dem der Benutzer in Authentik angelegt wird. Standard: `users` |

## Verwendung

1. Programm starten (unter Linux/macOS ggf. über das Terminal)
2. Das Tool fragt nacheinander nach:
   - Pfad zur CSV-Datei
   - Authentik-URL
   - API-Token
   - Gruppen-ID
   - Name der E-Mail-Stage
   - Ob Recovery-Links per E-Mail versendet werden sollen
3. Bereits bekannte Werte werden in Klammern angezeigt; Enter ohne Eingabe übernimmt den angezeigten Wert
4. Nach Eingabe aller Parameter wird eine Zusammenfassung angezeigt und um Bestätigung gebeten (`y`/`n`)
5. Nach Bestätigung verarbeitet das Tool die Liste und zeigt den Fortschritt live an
6. Am Ende erscheint eine Zusammenfassung mit Gesamtzahl, Erfolgen, Fehlern sowie den final vergebenen Benutzernamen

## FAQ

**Was passiert, wenn ein Benutzername bereits existiert?**
Das Tool erkennt dies automatisch und schlägt einen alternativen Benutzernamen vor, bis ein freier gefunden wird.

**Was passiert, wenn einzelne Benutzer fehlschlagen?**
Der Import läuft für die übrigen Einträge weiter. Fehlgeschlagene Einträge werden in der Abschlusszusammenfassung mit Fehlermeldung aufgeführt.

**Kann ein abgebrochener Import fortgesetzt werden?**
Aktuell nicht automatisch. Bereits erfolgreich angelegte Benutzer bleiben in Authentik bestehen und sollten vor einem erneuten Lauf aus der CSV-Liste entfernt werden, um doppelte Einträge zu vermeiden.

## Beitragen

Fehler oder Verbesserungsvorschläge können gerne als [Issue](../../issues) gemeldet werden.

## Lizenz

<!-- Lizenz hier ergänzen -->

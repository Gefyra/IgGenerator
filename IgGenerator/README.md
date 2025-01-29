<h1>Anleitung (Deutsch)</h1>

Im Ordner 'Publish' sind zwei ausführbare Dateien zu finden, jeweils für Windows und für Linux.

Es ist egal, wo das Tooling ausgeführt wird, es werden alle Pfade als Konsolen-Input abgefragt. Am Ablageort erstellt 
das Tool eine cache.xml, wo die Eingaben gespeichert werden für den nächsten Durchlauf.

<h2>Eingaben</h2>

- Use Cache?
  - (true|y|t|yes) oder (false|n|f|no) für ja oder nein
  - Bei 'Ja' werden die Eingaben aus der cache.xml verwendet. In der cache.xml nicht vorhandene Abfragen erscheinen trotzdem.
  - Bei 'Nein' werden alle Angaben nochmal abgefragt
- Do you want to manipulate names? (default: false)
  - (true|y|t|yes) oder (false|n|f|no) für ja oder nein
  - Bei 'Ja' kommt eine weitere Abfrage für die Filterung von Teilen aus Dateinamen und Überschriften
  - Bei 'Nein' werden die Namen so übernommen, wie sie aus dem Projekt kommen
- Path of Resources:
  - Eingabe des absoluten Pfades zu den Fhir-json Dateien des Projektes
  - Beispiel: C:\Users\beispiel-user\Documents\Spielwiese\isik-terminplanung-v4\Resources\fsh-generated\resources
  - Es können Warnungen kommen, wenn für ein Canonical aus dem Projekt keine passende StructureDefinition gefunden wird
- Select IgFolder Path:
  - Eingabe des absoluten Pfades zum IG, in dem das Template angewendet werden soll
  - Beispiel: C:\Users\beispiel-user\Documents\Spielwiese\isik-terminplanung-v4\guide\Implementierungsleitfaden_ISiKTerminplanung_401
  - Es wird ein Unterordner Datenobjekte angelegt, falls nicht vorhanden
  - Pro erstellter Datei gibt es eine Ausgabe
- Part to filter from filename (comma-separated list - no whitespace!):
  - Eingabe einer kommaseparierten Liste ohne Leerzeichen, welche Teile aus Dateinamen und Überschriften entfernt werden sollen
  - Beispiel: ISiK
    - So wird aus 'Datenobjekt_ISiKNachricht' der Ordner 'Datenobjekt_Nachricht'

<h2>Cache</h2>

Der Cache wird immer geschrieben und auch bei jedem Durchgang überschrieben, wenn er nicht benutzt wird. Es ist möglich, die 
Eingaben in der Cache.xml anzupassen, hierbei erfolgt aber aktuell keine Prüfung auf korrekte Antworten. Die Anwendung kann 
mit einem Fehler aussteigen.
Wird das Tool mit mehreren IGs verwendet, ist der einfachste Weg aktuell, die cache.xml umzubenennen und jeweils die aktuell 
benötigte als cache.xml zu benennen.
# Grueezi.AI
Grüezi.AI ist eine Beispiel-App die Azure Conitive Service nutzt um 
- Sprache in Text umzuwandeln
- Text als Sprache auszugeben
  
Grüezi.AI verwendet das neu allgemein verfügbare Sprachmodel für **Schweizerdeutsch**
und die beiden neuronalen Stimmen 
- Leni Deutsch Schweiz weiblich 
- Jan Deutsch männlich 


## Quickstart

App lokal ausführen

1. Zip File aus dem Repository herunterladen
2. In eine Ordner entpacken 
3. Das Install.ps mit Powershell ausführen
4. Die Meldungen mit ja bestätigen
5. Wenn alles erfolgreich ausgeführt wurde gibt es im Startmenü eine neue App "Grüezi.AI

App aus Visual Studio 

### Voraussetzungen

Für den Speech Service wird ein Subscription Key benötigt. Siehe [Try the speech service for free](https://docs.microsoft.com/azure/cognitive-services/speech-service/get-started).
* Ein Windows PC mit Windows 10 Fall Creators Update (10.0; Build 16299) oder höher und ein funktionierendes Mikrofon.
* [Microsoft Visual Studio (2017, 2019, 2022)](https://www.visualstudio.com/), Community Edition or higher.
* Mit **Universal Windows Platform development** Entwickliungstools in Visual Studio.
* Anmerkung: ARM or ARM64 Prozessoren werden nicht unterstützt

 ### Erstellen der App

* **Mit dem builden der App wird das Microsoft Cognitive Services Speech SDK herunter geladen. Mit dem Herunterladen erkennen sie die Microsoft Lizenz an [Speech SDK license agreement](https://aka.ms/csspeech/license201809).**
* [Laden Sie den Beispielcode auf Ihren Entwicklungs-PC herunter.](https://github.com/Trivadis/Grueezi.AI)
* Öffnen sie die "Grueezi.AI.sln" im Ordner "Source"
* Bearbeiten Sie den Quelltext "MainPage.xaml.cs":
  * Ersetzen Sie die Zeichenfolge `YourSubscriptionKey` in Zeile 32 durch Ihren eigenen Abonnementschlüssel.
  * Ersetzen Sie die Zeichenkette `YourServiceRegion` durch die Service-Region Ihres Abonnements.
    Ersetzen Sie zum Beispiel `switzerlandnorth`, wenn Sie das kostenlose 30-Tage-Testabonnement in der Region "Schweiz Nord" verwenden.


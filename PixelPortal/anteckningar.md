# Saker att fråga Anders

Vissa klasser har Update(delta) och viss bara Update() nvm

klassstruktur, hur kommer man på bra sätt at strukturera vad som ska göras av vilken klass? 
är det bättre med färre referenser, att inte alla klasser refererar till alla utan att ha det mer centrerat
till exempel portalhandler klassen kunda varit delvis integrerad med tilmap klasssen eller till och med helt samma klass, nu böhöver alla fysikentiteter ha varsin referens till PortalHandler vilket kanske är negativt.

hur är bästa sättet att göra med klasser som det bara ska finnas en av? jag har blandat massa olika sätt hitils: 
- Mathlike = statisk klass 
- Tilemap och SoundHandler = vanlig klass med delvis statiska saker i 
- Input = en statisk referens i Game1 och låta alla komma åt klassen därifrån 
- physics = en icke statisk referens i Game1 

Det här sättet käns väldigt rörigt (att blanda olika sätt så mycket). 

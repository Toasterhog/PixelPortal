vart jag fått inspiration från och liknande:



inspiration från monogame dokumentationen

* entity klassen är inspirerad av sprite klassen (innehåller variablerna som används i spritebatch.Draw())
* delar av input klassen är direkt kopierat från monogames input klass



inspirerat av godot / unity:

* strukturen att ha fysikrelaterade saker relativt separerat från andra saker är inspirerat från hur de spelmotorerna är strukturerade. ( t.ex. physicalentity, physicsUpdate(), physics klass som sköter physicalentities)



vart jag använt AI.

* för att lära mig saker som nya datatyper, syuntax och liknande
* de första iterationerna av collisionsdetektionen i physicalEntity
* mycket runt genererandet av lightTileSet-texturen (finns nu i temporarystuff klassen längst ner i game1-filen), och hur tilemap använder den (CLTS.png står för compact light tile set. kompakt för att de första texturerna var 16x16 tiles istället för 4x4.)
* läsa och spara filer (i nuläget bara .png) gjordes med AI. sen har jag läst och försökt fatta så mycket jag kan av det för att kunna andra saker i det vilket jag gjort. dock känner jag inte att jag har så bra koll på streams.





jag tror att jag hittade att interfaces fanns via Youtube när jag letade efter något annat, och märkte att de nog var perfekta för att kunna rita och uppdatera olika klasser som inte innheritar något gemensamt.



ljudeffekterna kommer från nåtet någonstans.

alla bilder har jag gjort själv utom bakgrunden som är från itch.io och companioncube som är från google.

hämis karaktären är inspirerad från ett monster/djur i spelet Noita.


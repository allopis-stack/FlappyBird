# Flappy Bird - Joviat

Versió del Flappy Bird que vam fer a classe amb les ampliacions de l'activitat.

Versió de Unity: 6000.3.25f1

L'APK per a Android és a la carpeta Build (Build/FlappyBird.apk).

## Controls

- Editor: espai o clic per començar i per saltar.
- Android: tocar la pantalla per començar i per saltar.
- També es pot començar amb el botó Start. Quan perds, el botó Restart torna a començar.

## Què he afegit

- Fons amb parallax que es repeteix sense talls i s'atura quan perds.
- Puntuació actual i rècord. El rècord es guarda amb PlayerPrefs.
- Diàleg de nou rècord quan superes el rècord anterior.
- Sons de salt, xoc, inici, punt i nou rècord.
- Animació de vol i de mort de l'ocell.
- L'ocell s'inclina cap amunt quan puja i cap avall quan cau.

## Paràmetres ajustats

- Salt: abans feia servir AddForce i el salt canviava segons si l'ocell estava caient o no. Ara fixo la velocitat cap amunt a 6.5 i cada salt és igual.
- Gravetat: Gravity Scale a 2.5, perquè l'ocell caigui més ràpid, com a l'original. També he limitat la velocitat de caiguda a 9.
- Canonades: van a 2.5 unitats per segon i en surt una parella cada 1.6 segons. El forat ara és de 2.8, abans era massa gran i era molt fàcil.
- Les canonades ara surten i desapareixen fora de la pantalla.
- He fet més petit el collider de l'ocell i he ajustat el de les canonades a la mida del sprite, perquè els xocs coincideixin amb el que es veu.
- El moviment es fa a FixedUpdate o amb Time.deltaTime, així va igual a l'editor i al mòbil encara que canviïn els FPS.
- Quan l'ocell mor ja no pot saltar ni sumar punts.

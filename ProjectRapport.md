# Projektrapport


Jag valda att arbeta på projektuppgiften själv. Jag har gjort en liten "restaurangservice" där man kan kan lägga in maträtter i en meny och kunder kan köpa dom olika rätterna.


LoginService låter en användare registrera sig och logga in. 
Servicen genererar sedan en JWT-Token när användaren loggat in. 

MenuService låter en admin ("Admin") lägga till olika maträtter i menyn. Den kan sedan ändra på en existerande maträtt eller ta bort den från listan.

PaymentService låter en användare köpa maträtter från menyn. Denna servicen kommunicerar med MenuService via http GET anrop.

Alla tjänsterna har sin egna databas.

Projektet köra lokalt via docker där alla tjänsterna har sina egna containers och databaser.

<br>

### Vad jag hade gjort för att fortsätta på uppgiften framåt
Jag skulle lägga till fler funktioner i dom olika endpointen. I registrering och login så skulle jag lägga till så att man inte kan registrera en redan existerande email. Jag skulle även lägga till funktioner så som att kunna banna en kund.




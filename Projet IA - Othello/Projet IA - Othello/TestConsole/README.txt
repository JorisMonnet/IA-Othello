ConsoleTestDLLOthello permet de v�rifier le fonctionnement des r�gles du jeu ArcOthello.
Votre controleur de jeu doit etre packag� comme une DLL dont le nom commence par "IA" avec une classe dont le nom contient "Board". Cette classe doit impl�menter l'interface IPlayable.Iplayable de l'assembly IPlayable.dll de ce dossier.

Param�tres du programme en ligne de commande (facultatifs):
args[0] : d�lai d'attente apr�s chaque coup [ms]   (0 --> validation par touche, 200 par d�faut)
args[1] : SHOW d�sactivera l'effacement de la console apr�s chaque coup  
P.ex.
c:\>    ConsoleTestDLLOthello  0 SHOW
c:\>    ConsoleTestDLLOthello  1000



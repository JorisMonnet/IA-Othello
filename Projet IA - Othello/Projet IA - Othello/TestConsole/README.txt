ConsoleTestDLLOthello permet de vérifier le fonctionnement des règles du jeu ArcOthello.
Votre controleur de jeu doit etre packagé comme une DLL dont le nom commence par "IA" avec une classe dont le nom contient "Board". Cette classe doit implémenter l'interface IPlayable.Iplayable de l'assembly IPlayable.dll de ce dossier.

Paramètres du programme en ligne de commande (facultatifs):
args[0] : délai d'attente après chaque coup [ms]   (0 --> validation par touche, 200 par défaut)
args[1] : SHOW désactivera l'effacement de la console après chaque coup  
P.ex.
c:\>    ConsoleTestDLLOthello  0 SHOW
c:\>    ConsoleTestDLLOthello  1000



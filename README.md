# IA-Othello

Notre fonction d'évaluation renvoit un entier(fitness) qui correspond à un poids d'une solution ou d'une autre dans notre arbre pour les trier ensuite.

Cette fonction utillise deux principes, un tableau où on donne des poids suivant la position du coup à évaluer et une fonction qui calcule le nombre de pions de l'adversaire qui seront retournés par notre coup.

Le tableau de poids change au cours de la partie. Tout d'abord, au début suivant si on est blanc ou noir on va mettre un poids supérieur sur notre coté le plus proche pour pousser le jeu sur un coté. En effet, il est avantageux de prendre les bords qui ne sont attaquables par l'adversaire que par le bord et pas par l'intérieur.

De manière général nos tableaux donnent un poids enorme au coins afin de pousser notre algorithme à y aller. Ensuite suivant le coin qu'on occupe, les poids changent pour essayer de prendre les bordures depuis ce coin. Si on possède aucoun coin, au bout de  itérations, on a un tableau général qui donne des poids évitant les colonnes et les lignes juste avant les lignes et colonnes de bordure et plus fortement les point qui entourent les coins pour éviter de les donner à notre adversaire.

Notre deuxième principe, est une fonction qui va calculer pour chaque coup qu'on veut faire le nombre de pions retournées par ce coup. En effet, plus un coup tourne de pions, plus il est bon en général. Cette fonction à le même poids que le tableau de poids, c'est a dire que fitness = fitness + cette fonction, comme avec les tableaux.

ADRIEN PAYSANT - JORIS MONNET
Equipe : GaletteSaucisse
# IA-Othello

Notre fonction d'évaluation renvoit un entier(fitness) qui correspond à un poids d'une feuille dans notre arbre pour les trier ensuite.

Cette fonction utilise deux principes, un tableau où on donne des poids suivant la position du coup à évaluer et une fonction qui calcule le nombre de pions de l'adversaire qui seront retournés par notre coup.

Le tableau de poids change au cours de la partie. Tout d'abord, si on est blanc ou noir on assigne un poids supérieur sur notre coté le plus proche pour pousser le jeu vers le   côté choisit. En effet, il est avantageux d'acquérir les bords qui ne sont pas attaquables par l'adversaire hormis le cas où l'adversaire est lui-même sur le bord considéré.

De manière générale nos tableaux allouent un poids conséquent aux coins. Cela afin de pousser notre algorithme à acquérir les coins. Dans un second temps, en fonction de la situaion d'un coin qu'on occupe, les poids changent pour essayer d'acquérir les bordures depuis ce coin. Si l'on ne possède aucun coin au bout de 10 itérations, le tableau donne des poids évitant les colonnes et les lignes juste avant les lignes et colonnes extérieures. Plus fortement les points qui entourent les coins possèdent des poids négatifs afin d'éviter de les donner à notre adversaire.

Notre deuxième principe, est une fonction qui va calculer pour chaque coup que l'on veut faire le nombre de pions retournées par ce coup à partir du 35ème tour (milieu approximatif de la partie). En effet, plus un coup tourne de pions, plus il est bon en général. 


ADRIEN PAYSANT - JORIS MONNET
Equipe : GaletteSaucisse

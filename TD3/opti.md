### 1. **Validation des entités avant `SaveChanges`**

- Utilise `ctx.ChangeTracker` pour voir l'état de chaque entité avant de les enregistrer.
- Appelle `ctx.GetValidationErrors()` pour récupérer les erreurs de validation avant d’essayer d’enregistrer.

### 2. **Gérer les Exceptions Spécifiques d'Entity Framework**

Voici quelques exceptions courantes à capturer :

- **`DbUpdateException`** : Déclenchée lors d'une erreur de mise à jour dans la base de données.
- **`DbEntityValidationException`** : Déclenchée lorsque les validations au niveau des entités échouent avant `SaveChanges()`.
- **`DbUpdateConcurrencyException`** : Déclenchée lorsque deux utilisateurs essaient de modifier la même ressource en même temps (concurrence).

**Utiliser des `try-catch` ciblés** : Gérer les exceptions propres à Entity Framework, comme `DbUpdateException` ou `DbUpdateConcurrencyException`, pour capturer les erreurs de clés étrangères, d’unicité, ou de concurrence.

### 3. **Validation côté client avec `TryValidateObject`**

Utiliser `Validator.TryValidateObject` pour tester les entités en amont peut éviter de nombreuses erreurs. Cela permet de vérifier les attributs comme `[Required]` ou `[Range]` avant de les envoyer à la base de données.

### **Test de Cas d'Erreur :**

1. **Commande sans client** : Crée une commande avec un `ClientId` non existant.
2. **Produit sans nom** : Insère un produit sans valeur pour `Name`.
3. **Quantité négative dans `OrderLine`** : Ajoute une ligne de commande avec une quantité négative.
4. **Violation de clé étrangère** : Tente d'insérer une `OrderLine` avec un `ProductId` non valide.

### **Résumé des bonnes pratiques** :

1. **Validation en amont des données** pour éviter les erreurs.
2. **Transactions pour garantir l'intégrité des opérations**.
3. **Gestion des exceptions spécifiques** pour un retour d’information clair.
4. **Activation des logs** pour mieux diagnostiquer les erreurs de base de données.

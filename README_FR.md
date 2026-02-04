# Monitor System - Moniteur Système

Un moniteur système .NET WPF qui affiche les métriques de performance en temps réel et génère des rapports PDF.

## Caractéristiques

### Surveillance en Temps Réel
- **Utilisation du CPU** : Affichée avec une jauge à aiguille de type compteur de vitesse (0-100%)
- **Utilisation de la RAM** : Graphique linéaire montrant l'utilisation de la mémoire dans le temps
- **Utilisation du Disque (ROM)** : Histogramme montrant l'espace utilisé et libre pour tous les lecteurs
- **Vitesse Réseau** : Vitesses de téléchargement et d'envoi en temps réel
- **Informations Système** : Version du système d'exploitation, nom de l'ordinateur, informations sur le processeur et temps de fonctionnement

### Enregistrement des Performances
- Cliquez sur "Start Recording" pour commencer la collecte de données de performance
- Cliquez sur "Stop Recording" pour générer un rapport PDF
- Le rapport PDF comprend :
  - Statistiques récapitulatives (min, max, moyenne pour CPU et RAM)
  - Tableau de données de performance détaillées avec 4 colonnes :
    1. Horodatage (Timestamp)
    2. Utilisation du CPU (%)
    3. Utilisation de la RAM (%)
    4. Vitesse réseau (téléchargement/envoi)
- Les rapports sont enregistrés sur votre Bureau

## Prérequis

### Prérequis .NET
- .NET 10.0 ou version ultérieure
- Système d'exploitation Windows (pour une prise en charge complète des compteurs de performance)

### Prérequis Python
- Python 3.7 ou version ultérieure
- Bibliothèque reportlab

## Installation

1. **Installer les dépendances Python :**
   ```bash
   cd SystemMonitor
   pip install -r requirements.txt
   ```

2. **Compiler l'application :**
   ```bash
   dotnet build
   ```

3. **Exécuter l'application :**
   ```bash
   dotnet run
   ```

## Utilisation

1. Lancez l'application pour voir la surveillance du système en temps réel
2. Pour enregistrer les performances :
   - Cliquez sur "Start Recording"
   - Laissez l'application fonctionner pendant la surveillance de votre système
   - Cliquez sur "Stop Recording" lorsque vous avez terminé
   - Le rapport PDF sera généré et enregistré sur votre Bureau

## Structure du Projet

```
SystemMonitor/
├── MainWindow.xaml          # Disposition de l'interface utilisateur
├── MainWindow.xaml.cs       # Logique de l'application
├── generate_report.py       # Script Python pour la génération de PDF
├── requirements.txt         # Dépendances Python
└── SystemMonitor.csproj     # Configuration du projet
```

## Technologies Utilisées

- **WPF (Windows Presentation Foundation)** : Framework d'interface utilisateur moderne
- **LiveCharts** : Bibliothèque de graphiques pour les jauges et les graphiques
- **PerformanceCounter** : Surveillance des performances du système
- **Python + ReportLab** : Génération de rapports PDF

## Détails de l'Implémentation

### Jauge à Aiguille pour le CPU
- Utilise LiveCharts Gauge
- Style compteur de vitesse comme demandé
- Plage de 0 à 100%
- Mise à jour chaque seconde

### Graphique pour la RAM
- Utilise LiveCharts LineChart
- Affiche l'historique des 30 dernières secondes
- Couleur orange pour la visibilité

### Histogramme pour le Disque (ROM)
- Utilise LiveCharts ColumnChart
- Affiche l'espace utilisé et libre
- Une colonne par lecteur
- Couleur violette

### Vitesse Réseau
- Affiche les vitesses de téléchargement et d'envoi
- Formatage automatique (KB/s ou MB/s)
- Mise à jour en temps réel

### Enregistrement PDF
- Script Python qui crée un PDF avec 4 colonnes comme demandé
- Comprend des statistiques récapitulatives
- Tableau détaillé avec toutes les données enregistrées
- Design professionnel avec couleurs et formatage

## Notes

- L'application fonctionne mieux sur les systèmes Windows où tous les compteurs de performance sont disponibles
- Sur les systèmes non-Windows, certaines fonctionnalités peuvent avoir des fonctionnalités limitées
- Assurez-vous que Python est installé et disponible dans votre PATH système pour que la génération de PDF fonctionne
- L'application se met à jour chaque seconde
- Les données d'enregistrement sont stockées temporairement dans un fichier CSV
- Les rapports PDF incluent des statistiques récapitulatives et des données détaillées

## Dépannage

### "Python introuvable"
- Assurez-vous que Python est installé et ajouté au PATH
- Testez en exécutant `python --version` dans le terminal

### Échec de la génération de PDF
- Assurez-vous que reportlab est installé : `pip install reportlab`
- Vérifiez que Python peut être appelé depuis la ligne de commande

### Les compteurs de performance ne fonctionnent pas
- Cette application fonctionne mieux sur Windows
- Sur Linux/Mac, certaines fonctionnalités peuvent avoir des fonctionnalités limitées

## Auteur

Développé pour le projet monitor-system de NicolasLBN

## Licence

Ce projet est open source et disponible sous licence MIT.

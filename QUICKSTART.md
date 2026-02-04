# Quick Start Guide - Guide de Démarrage Rapide

## English Version

### Prerequisites
1. Install .NET SDK 10.0+ from https://dotnet.microsoft.com/download
2. Install Python 3.7+ from https://www.python.org/downloads/

### Installation (3 Simple Steps)

#### Step 1: Install Python Dependencies
```bash
cd SystemMonitor
pip install reportlab
```

#### Step 2: Build the Application
```bash
dotnet build
```

#### Step 3: Run the Application
```bash
dotnet run
```

Or double-click: `SystemMonitor/bin/Debug/net10.0-windows/SystemMonitor.exe`

### Using the Application

1. **View Real-Time Monitoring**
   - CPU: Gauge shows current usage
   - RAM: Graph shows last 30 seconds
   - Disk: Histogram shows all drives
   - Network: Real-time speeds

2. **Record Performance Data**
   - Click "Start Recording" button
   - Wait while monitoring your system
   - Click "Stop Recording" button
   - PDF report opens automatically on your Desktop

### That's It!
The application is ready to use. All monitoring starts automatically when you launch the app.

---

## Version Française

### Prérequis
1. Installer .NET SDK 10.0+ depuis https://dotnet.microsoft.com/download
2. Installer Python 3.7+ depuis https://www.python.org/downloads/

### Installation (3 Étapes Simples)

#### Étape 1 : Installer les Dépendances Python
```bash
cd SystemMonitor
pip install reportlab
```

#### Étape 2 : Compiler l'Application
```bash
dotnet build
```

#### Étape 3 : Exécuter l'Application
```bash
dotnet run
```

Ou double-cliquez sur : `SystemMonitor/bin/Debug/net10.0-windows/SystemMonitor.exe`

### Utilisation de l'Application

1. **Voir la Surveillance en Temps Réel**
   - CPU : La jauge affiche l'utilisation actuelle
   - RAM : Le graphique montre les 30 dernières secondes
   - Disque : L'histogramme montre tous les lecteurs
   - Réseau : Vitesses en temps réel

2. **Enregistrer les Données de Performance**
   - Cliquez sur le bouton "Start Recording"
   - Attendez pendant la surveillance de votre système
   - Cliquez sur le bouton "Stop Recording"
   - Le rapport PDF s'ouvre automatiquement sur votre Bureau

### C'est Tout !
L'application est prête à l'emploi. Toute la surveillance démarre automatiquement lorsque vous lancez l'application.

---

## Troubleshooting / Dépannage

### Problem: "Python not found"
**Solution**: Add Python to your system PATH or use full path to python.exe

### Problème : "Python introuvable"
**Solution** : Ajoutez Python à votre PATH système ou utilisez le chemin complet vers python.exe

### Problem: PDF generation fails
**Solution**: Run `pip install reportlab` again

### Problème : La génération de PDF échoue
**Solution** : Exécutez `pip install reportlab` à nouveau

### Problem: Build errors
**Solution**: Make sure .NET SDK 10.0 is installed, run `dotnet --version`

### Problème : Erreurs de compilation
**Solution** : Assurez-vous que .NET SDK 10.0 est installé, exécutez `dotnet --version`

---

## Features Summary / Résumé des Fonctionnalités

✅ CPU monitoring with speedometer gauge / Surveillance CPU avec jauge type compteur  
✅ RAM monitoring with real-time graph / Surveillance RAM avec graphique temps réel  
✅ Disk monitoring with histogram / Surveillance disque avec histogramme  
✅ Network speed monitoring / Surveillance vitesse réseau  
✅ System information display / Affichage informations système  
✅ PDF report generation / Génération de rapports PDF  
✅ 4-column data table in PDF / Tableau de données à 4 colonnes dans PDF  
✅ Summary statistics / Statistiques récapitulatives  
✅ Dark theme UI / Interface thème sombre  
✅ Professional design / Design professionnel  

---

## Need More Help? / Besoin d'Aide ?

See full documentation:
- `README.md` - Main documentation (English)
- `README_FR.md` - Documentation principale (Français)
- `SETUP_GUIDE.md` - Detailed setup (English)
- `IMPLEMENTATION.md` - Technical details (Bilingual)
- `VISUAL_GUIDE.md` - UI description (English)

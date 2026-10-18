@echo off
:: Noxticord Installer — Lance l'interface graphique portable
:: Aucun prérequis : PowerShell est déjà présent sur Windows
title Noxticord Installer
powershell.exe -ExecutionPolicy Bypass -File "%~dp0installer-gui.ps1"

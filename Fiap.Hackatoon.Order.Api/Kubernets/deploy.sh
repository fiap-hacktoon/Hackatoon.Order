#!/bin/bash

set -e  # Encerra o script em caso de erro

echo "🔐 Aplicando Secret da aplicação..."
kubectl apply -f App-secrets.yaml  # Adiciona o Secret

echo "🐬 Subindo MySQL Deployment..."
kubectl apply -f MySqlDeployment.yaml  # Aplica o MySQL Deployment

echo "🌐 Subindo MySQL Service..."
kubectl apply -f MySqlService.yaml  # Aplica o MySQL Service

echo "⚙️ Aplicando ConfigMap da aplicação..."
kubectl apply -f Configmap.yaml  # Aplica o ConfigMap

echo "📦 Aplicando Deployment da aplicação..."
kubectl apply -f Deployment.yaml  # Aplica o Deployment da aplicação

echo "🚪 Aplicando Service da aplicação..."
kubectl apply -f Service.yaml  # Aplica o Service da aplicação

# Aplicando o HPA
echo "📈 Aplicando Horizontal Pod Autoscaler (HPA)..."
kubectl apply -f hpa.yaml  # Aplica o HPA a partir do arquivo hpa.yaml

echo "✅ Tudo aplicado com sucesso!"
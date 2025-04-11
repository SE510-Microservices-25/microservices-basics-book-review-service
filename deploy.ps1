param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("start", "stop", "url")]
    [string]$Action
)

function Build-DockerImage {
    Write-Host "Building Docker images..." -ForegroundColor Cyan
    $minikubeEnv = minikube docker-env --shell powershell
    if ($?) {
        Write-Host "Setting Minikube Docker environment" -ForegroundColor Yellow
        & minikube -p minikube docker-env --shell powershell | Invoke-Expression
    }
    docker build --network=host -t review-service:latest -f ReviewService/Dockerfile ReviewService/
}

function Deploy-ToKubernetes {
    Write-Host "Deploying to Kubernetes..." -ForegroundColor Cyan
    kubectl apply -f k8s/postgres-secret.yaml
    kubectl apply -f k8s/service-secret.yaml
    kubectl apply -f k8s/keycloak-pvc.yaml

    # Services
    kubectl apply -f k8s/postgres-service.yaml
    kubectl apply -f k8s/keycloak-service.yaml
    kubectl apply -f k8s/review-service.yaml
    kubectl apply -f k8s/rabbitmq-service.yaml

    # Deployments
    kubectl apply -f k8s/postgres-deployment.yaml
    kubectl apply -f k8s/keycloak-deployment.yaml
    kubectl apply -f k8s/rabbitmq-deployment.yaml
    kubectl apply -f k8s/review-deployment.yaml

    # Ingress
    kubectl apply -f k8s/ingress.yaml
}

function Wait-ForPodsReady {
    Write-Host "Waiting for pods to be ready..." -ForegroundColor Yellow
    foreach ($i in 1..10) {
        $podStatus = kubectl get pods --no-headers | Where-Object { $_ -notmatch "Running|Completed" }
        if (-not $podStatus) {
            Write-Host "All pods are ready!" -ForegroundColor Green
            return $true
        }

        Start-Sleep -Seconds 6
    }

    Write-Host "Timeout waiting for pods to be ready" -ForegroundColor Red
    return $false
}

function Get-ServiceUrl {
    $ingressHost = kubectl get ingress -o jsonpath='{.items[0].spec.rules[0].host}'

    if ($ingressHost) {
        Write-Host "Service URL:" -ForegroundColor Cyan
        Write-Host "http://$ingressHost/swagger" -ForegroundColor Green

        Write-Host "Keycloak URL:" -ForegroundColor Cyan
        Write-Host "http://auth.$ingressHost" -ForegroundColor Green
    } else {
        Write-Host "Ingress host not found." -ForegroundColor Yellow
    }
}

function Cleanup-Cluster {
    Write-Host "Cleaning up Kubernetes cluster..."
    # Ingress
    kubectl delete -f k8s/ingress.yaml

    # Deployments
    kubectl delete -f k8s/review-deployment.yaml
    kubectl delete -f k8s/rabbitmq-deployment.yaml
    kubectl delete -f k8s/keycloak-deployment.yaml
    kubectl delete -f k8s/postgres-deployment.yaml

    # Services
    kubectl delete -f k8s/review-service.yaml
    kubectl delete -f k8s/rabbitmq-service.yaml
    kubectl delete -f k8s/keycloak-service.yaml
    kubectl delete -f k8s/postgres-service.yaml

    kubectl delete -f k8s/postgres-secret.yaml
    kubectl delete -f k8s/service-secret.yaml
    #    kubectl delete -f k8s/keycloak-pvc.yaml
}

switch ($Action) {
    "start" {
        Build-DockerImage
        Deploy-ToKubernetes
        if (Wait-ForPodsReady) {
            kubectl get pods
            Get-ServiceUrl
        }
    }
    "stop" {
        Cleanup-Cluster
    }
    "url" {
        Get-ServiceUrl
    }
}

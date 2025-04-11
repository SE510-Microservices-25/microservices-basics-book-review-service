param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("start", "stop", "url")]
    [string]$Action
)

function Build-DockerImage {
    Write-Host "Building Review Service image..." -ForegroundColor Cyan
    $minikubeEnv = minikube docker-env --shell powershell
    if ($?) {
        Write-Host "Setting Minikube Docker environment" -ForegroundColor Yellow
        & minikube -p minikube docker-env --shell powershell | Invoke-Expression
    }

    docker build --network=host -t review-service:latest -f ReviewService/Dockerfile ReviewService/
}

function Deploy-ToKubernetes {
    Write-Host "Deploying Review Service to Kubernetes..." -ForegroundColor Cyan

    # Apply RabbitMQ
    kubectl apply -f k8s/review/rabbitmq/deployment.yaml
    kubectl apply -f k8s/review/rabbitmq/service.yaml

    # Apply PostgreSQL
    kubectl apply -f k8s/review/postgres/deployment.yaml
    kubectl apply -f k8s/review/postgres/service.yaml
    kubectl apply -f k8s/review/postgres/secret.yaml

    # Apply Keycloak
    kubectl apply -f k8s/review/keycloak/pvc.yaml
    kubectl apply -f k8s/review/keycloak/deployment.yaml
    kubectl apply -f k8s/review/keycloak/service.yaml

    # Apply Review Service
    kubectl apply -f k8s/review/review-service/secret.yaml
    kubectl apply -f k8s/review/review-service/deployment.yaml
    kubectl apply -f k8s/review/review-service/service.yaml

    # Apply Ingress
    kubectl apply -f k8s/review/ingress.yaml
}

function Wait-ForPodsReady {
    Write-Host "Waiting for Review Service pods to be ready..." -ForegroundColor Yellow
    $labelSelector = "app in (review-service,review-postgres,review-rabbitmq,review-keycloak)"
    foreach ($i in 1..10) {
        $podStatus = kubectl get pods --selector=$labelSelector --no-headers | Where-Object { $_ -notmatch "Running|Completed" }
        if (-not $podStatus) {
            Write-Host "All Review Service pods are ready!" -ForegroundColor Green
            return $true
        }
        Start-Sleep -Seconds 6
    }

    Write-Host "Timeout waiting for Review Service pods to be ready" -ForegroundColor Red
    return $false
}

function Get-ServicesUrl {
    Write-Host "Review Service: http://review.local/swagger" -ForegroundColor Green
    Write-Host "Keycloak: http://auth.review.local" -ForegroundColor Green
    Write-Host "RabbitMQ Management: http://review-rabbitmq.local" -ForegroundColor Green
}

function Cleanup-Cluster {
    Write-Host "Cleaning up Review Service from Kubernetes cluster..." -ForegroundColor Cyan

    # Delete Ingress
    kubectl delete -f k8s/review/ingress.yaml

    # Delete Review Service
    kubectl delete -f k8s/review/review-service/service.yaml
    kubectl delete -f k8s/review/review-service/deployment.yaml
    kubectl delete -f k8s/review/review-service/secret.yaml

    # Delete Keycloak
    kubectl delete -f k8s/review/keycloak/service.yaml
    kubectl delete -f k8s/review/keycloak/deployment.yaml

    # Delete PostgreSQL
    kubectl delete -f k8s/review/postgres/secret.yaml
    kubectl delete -f k8s/review/postgres/service.yaml
    kubectl delete -f k8s/review/postgres/deployment.yaml

    # Delete RabbitMQ
    kubectl delete -f k8s/review/rabbitmq/service.yaml
    kubectl delete -f k8s/review/rabbitmq/deployment.yaml
}

switch ($Action) {
    "start" {
        Build-DockerImage
        Deploy-ToKubernetes
        if (Wait-ForPodsReady) {
            kubectl get pods -l "app in (review-service,review-postgres,review-rabbitmq,review-keycloak)"
            Get-ServicesUrl
        }
    }
    "stop" {
        Cleanup-Cluster
    }
    "url" {
        Get-ServicesUrl
    }
}

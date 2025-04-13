param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("start", "stop", "url")]
    [string]$Action
)

function Build-DockerImage {
    Write-Host "Building Book Service image..." -ForegroundColor Cyan
    $minikubeEnv = minikube docker-env --shell powershell
    if ($?) {
        Write-Host "Setting Minikube Docker environment" -ForegroundColor Yellow
        & minikube -p minikube docker-env --shell powershell | Invoke-Expression
    }

    docker build --network=host -t book-service:latest -f BookService/Dockerfile BookService/
}

function Deploy-ToKubernetes {
    Write-Host "Deploying Book Service to Kubernetes..." -ForegroundColor Cyan
    
    # Common
    kubectl apply -f k8s/common/rabbitmq/service.yaml
    kubectl apply -f k8s/common/rabbitmq/deployment.yaml
    kubectl apply -f k8s/common/ingress.yaml

    # Secrets
    kubectl apply -f k8s/book/postgres/secret.yaml
    kubectl apply -f k8s/book/book-service/secret.yaml

    # Services
    kubectl apply -f k8s/book/postgres/service.yaml
    kubectl apply -f k8s/book/book-service/service.yaml

    # Deployments
    kubectl apply -f k8s/book/postgres/deployment.yaml
    kubectl apply -f k8s/book/book-service/deployment.yaml

    # Ingress
    kubectl apply -f k8s/book/ingress.yaml
}

function Wait-ForPodsReady {
    Write-Host "Waiting for Book Service pods to be ready..." -ForegroundColor Yellow
    $labelSelector = "app in (book-service,book-postgres,book-rabbitmq)"
    foreach ($i in 1..10) {
        $podStatus = kubectl get pods --selector=$labelSelector --no-headers | Where-Object { $_ -notmatch "Running|Completed" }
        if (-not $podStatus) {
            Write-Host "All Book Service pods are ready!" -ForegroundColor Green
            return $true
        }
        Start-Sleep -Seconds 6
    }

    Write-Host "Timeout waiting for Book Service pods to be ready" -ForegroundColor Red
    return $false
}

function Get-ServicesUrl {
    Write-Host "Book Service: http://book.local/swagger" -ForegroundColor Green
    Write-Host "RabbitMQ Management: http://rabbitmq.local" -ForegroundColor Green
}

function Cleanup-Cluster {
    Write-Host "Cleaning up Book Service from Kubernetes cluster..." -ForegroundColor Cyan

    # Ingress
    kubectl delete -f k8s/book/ingress.yaml

    # Deployments
    kubectl delete -f k8s/book/book-service/deployment.yaml
    kubectl delete -f k8s/book/postgres/deployment.yaml

    # Services
    kubectl delete -f k8s/book/book-service/service.yaml
    kubectl delete -f k8s/book/postgres/service.yaml

    # Secrets
    kubectl delete -f k8s/book/book-service/secret.yaml
    kubectl delete -f k8s/book/postgres/secret.yaml
    
    # Common
    kubectl delete -f k8s/common/ingress.yaml
    kubectl delete -f k8s/common/rabbitmq/deployment.yaml
    kubectl delete -f k8s/common/rabbitmq/service.yaml
}

switch ($Action) {
    "start" {
        Build-DockerImage
        Deploy-ToKubernetes
        if (Wait-ForPodsReady) {
            kubectl get pods -l "app in (book-service,book-postgres,book-rabbitmq)"
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

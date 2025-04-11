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

    # Apply RabbitMQ
    kubectl apply -f k8s/book/rabbitmq/deployment.yaml
    kubectl apply -f k8s/book/rabbitmq/service.yaml

    # Apply PostgreSQL
    kubectl apply -f k8s/book/postgres/deployment.yaml
    kubectl apply -f k8s/book/postgres/service.yaml
    kubectl apply -f k8s/book/postgres/secret.yaml

    # Apply Book Service
    kubectl apply -f k8s/book/book-service/secret.yaml
    kubectl apply -f k8s/book/book-service/deployment.yaml
    kubectl apply -f k8s/book/book-service/service.yaml

    # Apply Ingress
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
    Write-Host "RabbitMQ Management: http://book-rabbitmq.local" -ForegroundColor Green
}

function Cleanup-Cluster {
    Write-Host "Cleaning up Book Service from Kubernetes cluster..." -ForegroundColor Cyan

    # Delete Ingress
    kubectl delete -f k8s/book/ingress.yaml

    # Delete Book Service
    kubectl delete -f k8s/book/book-service/service.yaml
    kubectl delete -f k8s/book/book-service/deployment.yaml
    kubectl delete -f k8s/book/book-service/secret.yaml

    # Delete PostgreSQL
    kubectl delete -f k8s/book/postgres/secret.yaml
    kubectl delete -f k8s/book/postgres/service.yaml
    kubectl delete -f k8s/book/postgres/deployment.yaml

    # Delete RabbitMQ
    kubectl delete -f k8s/book/rabbitmq/service.yaml
    kubectl delete -f k8s/book/rabbitmq/deployment.yaml
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

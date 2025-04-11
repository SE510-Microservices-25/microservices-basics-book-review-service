param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("start", "stop", "url")]
    [string]$Action
)

function Build-DockerImages {
    Write-Host "Building Docker images..." -ForegroundColor Cyan
    $minikubeEnv = minikube docker-env --shell powershell
    if ($?) {
        Write-Host "Setting Minikube Docker environment" -ForegroundColor Yellow
        & minikube -p minikube docker-env --shell powershell | Invoke-Expression
    }

    Write-Host "Building Book Service image..." -ForegroundColor Cyan
    docker build --network=host -t book-service:latest -f BookService/Dockerfile BookService/

    Write-Host "Building Review Service image..." -ForegroundColor Cyan
    docker build --network=host -t review-service:latest -f ReviewService/Dockerfile ReviewService/
}

function Deploy-ToKubernetes {
    Write-Host "Deploying to Kubernetes..." -ForegroundColor Cyan
    kubectl apply -f k8s/service-secret.yaml
    kubectl apply -f k8s/rabbitmq-deployment.yaml
    kubectl apply -f k8s/rabbitmq-service.yaml

    # Book
    kubectl apply -f k8s/book/postgres-deployment.yaml
    kubectl apply -f k8s/book/book-service.yaml
    kubectl apply -f k8s/book/book-deployment.yaml
    
    # Review
    kubectl apply -f k8s/review/postgres-secret.yaml
    kubectl apply -f k8s/review/keycloak-pvc.yaml
    
    # Services
    kubectl apply -f k8s/review/postgres-service.yaml
    kubectl apply -f k8s/review/keycloak-service.yaml
    kubectl apply -f k8s/review/review-service.yaml

    # Deployments
    kubectl apply -f k8s/review/postgres-deployment.yaml
    kubectl apply -f k8s/review/keycloak-deployment.yaml
    kubectl apply -f k8s/review/review-deployment.yaml

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

function Get-ServicesUrl {
    $ingressHost = kubectl get ingress -o jsonpath='{.items[0].spec.rules[0].host}'

    if ($ingressHost) {
        Write-Host "Book Service: http://book.$ingressHost/swagger" -ForegroundColor Green
        Write-Host "Review Service: http://review.$ingressHost/swagger" -ForegroundColor Green
        Write-Host "Combined API: http://$ingressHost" -ForegroundColor Green
        Write-Host "RabbitMQ Management: http://rabbitmq.$ingressHost" -ForegroundColor Green
        Write-Host "Keycloak: http://auth.$ingressHost" -ForegroundColor Green
    } else {
        Write-Host "Ingress host not found." -ForegroundColor Yellow
    }
}

function Cleanup-Cluster {
    Write-Host "Cleaning up Kubernetes cluster..."
    # Ingress
    kubectl delete -f k8s/ingress.yaml

    # Deployments
    kubectl apply -f k8s/review/review-deployment.yaml
    kubectl apply -f k8s/review/keycloak-deployment.yaml
    kubectl apply -f k8s/review/postgres-deployment.yaml

    # Services
    kubectl delete -f k8s/review/review-service.yaml
    kubectl delete -f k8s/review/keycloak-service.yaml
    kubectl delete -f k8s/review/postgres-service.yaml
    kubectl delete -f k8s/review/postgres-secret.yaml

    # Book
    kubectl delete -f k8s/book/book-deployment.yaml
    kubectl delete -f k8s/book/book-service.yaml
    kubectl delete -f k8s/book/postgres-deployment.yaml
    
    # Common
    kubectl delete -f k8s/rabbitmq-service.yaml
    kubectl delete -f k8s/rabbitmq-deployment.yaml
    kubectl delete -f k8s/service-secret.yaml
}

switch ($Action) {
    "start" {
        Build-DockerImages
        Deploy-ToKubernetes
        if (Wait-ForPodsReady) {
            kubectl get pods
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

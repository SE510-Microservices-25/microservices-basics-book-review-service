param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("start", "stop", "status", "url")]
    [string]$Action
)

function Build-DockerImage {
    Write-Host "Building Docker images..." -ForegroundColor Cyan
    docker build -t review-service:latest -f ReviewService/Dockerfile ReviewService/
}

function Deploy-ToKubernetes {
    Write-Host "Deploying to Kubernetes..." -ForegroundColor Cyan
    kubectl apply -f k8s/postgres-secret.yaml
    kubectl apply -f k8s/postgres-service.yaml
    kubectl apply -f k8s/postgres-deployment.yaml
    kubectl apply -f k8s/review-service.yaml
    kubectl apply -f k8s/review-deployment.yaml
    kubectl apply -f k8s/ingress.yaml
}

function Wait-ForPodsReady {
    param (
        [int]$TimeoutSeconds = 300
    ) 
    
    Write-Host "Waiting for pods to be ready..." -ForegroundColor Yellow
    $startTime = Get-Date
    while ((Get-Date) -lt $startTime.AddSeconds($TimeoutSeconds)) {
        $podStatus = kubectl get pods --no-headers | Where-Object { $_ -notmatch "Running|Completed" }
        if (-not $podStatus) {
            Write-Host "All pods are ready!" -ForegroundColor Green
            return $true
        }
        Start-Sleep -Seconds 2
    }

    Write-Host "Timeout waiting for pods to be ready" -ForegroundColor Red
    return $false
}

function Check-PodStatus {
    Write-Host "Checking pod status..." -ForegroundColor Cyan
    kubectl get pods
}

function Get-ServiceUrl {
    $ingressHost = kubectl get ingress -o jsonpath='{.items[0].spec.rules[0].host}'
        
    if ($ingressHost) {
        Write-Host "Service URL:" -ForegroundColor Cyan
        Write-Host "http://$ingressHost/swagger" -ForegroundColor Green
    } else {
        Write-Host "Ingress host not found." -ForegroundColor Yellow
    }
}

function Cleanup-Cluster {
    Write-Host "Cleaning up Kubernetes cluster..."
    kubectl delete -f k8s/ingress.yaml
    kubectl delete -f k8s/review-deployment.yaml
    kubectl delete -f k8s/review-service.yaml
    kubectl delete -f k8s/postgres-deployment.yaml
    kubectl delete -f k8s/postgres-service.yaml
    kubectl delete -f k8s/postgres-secret.yaml
}

switch ($Action) {
    "start" {
        Build-DockerImage
        Deploy-ToKubernetes
        if (Wait-ForPodsReady) {
            Check-PodStatus
            Get-ServiceUrl
        }
    }
    "stop" {
        Cleanup-Cluster
    }
    "status" {
        Check-PodStatus
    }
    "url" {
        Get-ServiceUrl
    }
}

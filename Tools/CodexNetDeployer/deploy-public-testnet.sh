dotnet run \
    --deploy-name=codex-public-testnet \
    --kube-config=/opt/kubeconfig.yaml \
    --kube-namespace=codex-public-testnet \
    --deploy-file=codex-public-testnet-deployment.json \
    --nodes=3 \
    --validators=1 \
    --log-level=Trace \
    --make-storage-available=0 \
    --block-ttl=180 \
    --block-mi=120 \
    --block-mn=10000 \
    --metrics-endpoints=1 \
    --metrics-scraper=0 \
    --check-connect=1 \
\
    --public-testnet=1 \
    --public-ip=1.2.3.4 \
    --public-discports=20010,20020,20030 \
    --public-listenports=20011,20021,20031

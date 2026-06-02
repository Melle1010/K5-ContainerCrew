# Docker Setup - Environment Variables

## ⚠️ Security Notice

This project uses environment variables for sensitive credentials. **Never commit `.env` files to Git.**

## Setup Instructions

### 1. Create `.env` file from example

```bash
cp .env.example .env
```

### 2. Edit `.env` with your actual secrets

```bash
# Edit .env and replace placeholder values with your real credentials
SERVICEB_API_KEY=your-actual-key
LLMPROXY_API_KEY=your-actual-key
GEMINI_API_KEY=your-actual-gemini-key
```

### 3. Start Docker containers

```bash
# Build and start all services
docker-compose up -d

# Or rebuild from scratch
docker-compose down && docker-compose build --no-cache && docker-compose up -d
```

### 4. Access Services

- **AiContent API**: http://localhost:5292
- **AiContent API Docs**: http://localhost:5292/scalar/v1
- **LlmProxy API**: http://localhost:5188

## Environment Variables Explained

| Variable | Service | Purpose |
|----------|---------|---------|
| `SERVICEB_API_KEY` | AiContent | API key for communicating with LlmProxy |
| `LLMPROXY_API_KEY` | LlmProxy | API key for service validation |
| `GEMINI_API_KEY` | LlmProxy | Google Gemini API key for AI generation |

## File Structure

```
.env              ← Your local secrets (NEVER commit)
.env.example      ← Template for secrets (safe to commit)
docker-compose.yml ← References .env variables
```

## Security Best Practices

✅ **DO:**
- Keep `.env` in `.gitignore`
- Use strong, unique API keys
- Rotate keys regularly
- Use `.env.example` as documentation

❌ **DON'T:**
- Commit `.env` to Git
- Share `.env` files via email/chat
- Use dummy keys in production
- Hardcode secrets in code

## Troubleshooting

### Container won't start
Check logs:
```bash
docker-compose logs -f aicontent
docker-compose logs -f llmproxy
```

### "Missing API key" error
Verify `.env` file exists and has correct values:
```bash
cat .env
```

### Changes not taking effect
Rebuild containers:
```bash
docker-compose down
docker-compose build --no-cache
docker-compose up -d
```

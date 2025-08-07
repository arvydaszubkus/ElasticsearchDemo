# 🧠 ElasticsearchDemo (C# Console App)

Perform **CRUD operations** with **Elasticsearch** using the **NEST client**.

- ✅ Create an Elasticsearch index
- 📥 Add documents (products)
- 🔍 Full-text search (`Match` query)
- ✏️ Update documents
- ❌ Delete documents
- 💬 Safe null-checks and modern C# practices

## 🐳 Run Elasticsearch with Docker

```bash
docker run -d -p 9201:9200 -e "discovery.type=single-node" -e "xpack.security.enabled=false" elasticsearch:8.13.0

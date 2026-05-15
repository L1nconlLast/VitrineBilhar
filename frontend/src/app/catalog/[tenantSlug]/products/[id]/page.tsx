"use client";

import Link from "next/link";
import { useParams } from "next/navigation";
import { useEffect, useState } from "react";

type ProductDto = {
  id: string;
  name: string;
  description: string | null;
  price: number;
  isActive: boolean;
  categoryId: string | null;
};

const API_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5155";

function formatBRL(value: number) {
  try {
    return value.toLocaleString("pt-BR", { style: "currency", currency: "BRL" });
  } catch {
    return `R$ ${Number(value).toFixed(2)}`;
  }
}

export default function CatalogProductDetailPage({
}: {}) {
  const params = useParams<{ tenantSlug: string; id: string }>();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [product, setProduct] = useState<ProductDto | null>(null);

  useEffect(() => {
    async function load() {
      setLoading(true);
      setError(null);
      try {
        const res = await fetch(
          `${API_URL}/api/catalog/${encodeURIComponent(params.tenantSlug)}/products/${encodeURIComponent(params.id)}`
        );

        if (res.status === 404) {
          setProduct(null);
          return;
        }

        if (!res.ok) throw new Error(`HTTP ${res.status}`);
        setProduct(await res.json());
      } catch (e: any) {
        setError(e?.message ?? "Erro ao carregar produto");
      } finally {
        setLoading(false);
      }
    }

    load();
  }, [params.tenantSlug, params.id]);

  return (
    <main className="mx-auto max-w-3xl px-4 py-8 space-y-6">
      <header className="flex items-center justify-between">
        <Link className="text-sm underline" href={`/catalog/${params.tenantSlug}`}>
          ← Voltar para o catálogo
        </Link>
        <Link className="text-sm underline" href="/admin/products">
          Admin (DEV)
        </Link>
      </header>

      {loading && <div>Carregando...</div>}

      {error && (
        <div className="text-red-700">
          Erro: {error}
          <div className="text-sm text-gray-600 mt-2">
            Confirme se a API está rodando em <code>{API_URL}</code>.
          </div>
        </div>
      )}

      {!loading && !error && !product && (
        <div className="text-gray-700">
          Produto não encontrado (talvez foi desativado ou deletado).
        </div>
      )}

      {product && (
        <section className="rounded-lg border bg-white p-6 space-y-3">
          <h1 className="text-2xl font-bold">{product.name}</h1>

          <div className="text-lg font-semibold">{formatBRL(product.price)}</div>

          {product.description ? (
            <p className="text-gray-700 whitespace-pre-wrap">{product.description}</p>
          ) : (
            <p className="text-gray-400 italic">Sem descrição.</p>
          )}

          <div className="text-xs text-gray-500">
            ID: <code>{product.id}</code>
          </div>
        </section>
      )}
    </main>
  );
}

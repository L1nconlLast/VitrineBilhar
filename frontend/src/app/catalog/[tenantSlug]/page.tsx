"use client";

import Link from "next/link";
import { useParams } from "next/navigation";
import { useEffect, useMemo, useState } from "react";

type ProductDto = {
  id: string;
  name: string;
  description: string | null;
  price: number;
  isActive: boolean;
  categoryId: string | null;
};

const API_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5155";
const PAGE_SIZE = 12;

function escapeRegExp(value: string) {
  return value.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
}

function Highlight({ text, query }: { text: string; query: string }) {
  const normalizedQuery = query.trim();
  if (!normalizedQuery) return <>{text}</>;

  const regex = new RegExp(`(${escapeRegExp(normalizedQuery)})`, "ig");
  const parts = text.split(regex);

  return (
    <>
      {parts.map((part, index) => {
        const isMatch = part.toLowerCase() === normalizedQuery.toLowerCase();
        return isMatch ? (
          <mark key={index} className="rounded bg-yellow-200 px-0.5">
            {part}
          </mark>
        ) : (
          <span key={index}>{part}</span>
        );
      })}
    </>
  );
}

function formatBRL(value: number) {
  try {
    return value.toLocaleString("pt-BR", { style: "currency", currency: "BRL" });
  } catch {
    return `R$ ${Number(value).toFixed(2)}`;
  }
}

export default function CatalogTenantPage({
}: {}) {
  const params = useParams<{ tenantSlug: string }>();
  const [items, setItems] = useState<ProductDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [search, setSearch] = useState("");
  const [debouncedSearch, setDebouncedSearch] = useState("");
  const [sort, setSort] = useState<"name" | "priceAsc" | "priceDesc">("name");
  const [page, setPage] = useState(1);

  useEffect(() => {
    const timer = setTimeout(() => setDebouncedSearch(search), 250);
    return () => clearTimeout(timer);
  }, [search]);

  useEffect(() => {
    async function load() {
      setLoading(true);
      setError(null);

      try {
        const res = await fetch(
          `${API_URL}/api/catalog/${encodeURIComponent(params.tenantSlug)}/products`
        );

        if (!res.ok) throw new Error(`HTTP ${res.status}`);

        setItems(await res.json());
      } catch (err: any) {
        setError(err?.message ?? "Erro ao carregar catálogo");
      } finally {
        setLoading(false);
      }
    }

    setPage(1);
    load();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [params.tenantSlug]);

  const normalizedSearch = debouncedSearch.trim().toLowerCase();

  const sorted = useMemo(() => {
    const filtered = items.filter((product) => {
      if (!normalizedSearch) return true;
      const haystack = `${product.name} ${product.description ?? ""}`.toLowerCase();
      return haystack.includes(normalizedSearch);
    });

    return [...filtered].sort((left, right) => {
      if (sort === "name") return left.name.localeCompare(right.name);
      if (sort === "priceAsc") return left.price - right.price;
      return right.price - left.price;
    });
  }, [items, normalizedSearch, sort]);

  const totalPages = Math.max(1, Math.ceil(sorted.length / PAGE_SIZE));
  const safePage = Math.min(Math.max(1, page), totalPages);

  const paged = useMemo(() => {
    const start = (safePage - 1) * PAGE_SIZE;
    return sorted.slice(start, start + PAGE_SIZE);
  }, [sorted, safePage]);

  useEffect(() => {
    setPage(1);
  }, [normalizedSearch, sort]);

  return (
    <main className="mx-auto max-w-6xl space-y-6 px-4 py-8">
      <header className="space-y-2">
        <div className="flex items-center justify-between gap-4">
          <h1 className="text-2xl font-bold">Catálogo — {params.tenantSlug}</h1>
          <div className="flex items-center gap-4">
            <Link className="text-sm underline" href="/admin/products">
              Ir para Admin (DEV)
            </Link>
            <Link className="text-sm underline" href="/">
              Home
            </Link>
          </div>
        </div>
        <p className="text-sm text-gray-600">Busca, ordenação e paginação com 12 itens por página.</p>
      </header>

      <section className="overflow-hidden rounded-lg border bg-white">
        <div className="border-b px-4 py-3">
          <div className="grid grid-cols-1 gap-3 md:grid-cols-3">
            <div className="md:col-span-2">
              <label className="block text-sm font-medium text-gray-700">Buscar</label>
              <div className="mt-1 flex gap-2">
                <input
                  value={search}
                  onChange={(event) => setSearch(event.target.value)}
                  placeholder="Ex: taco, giz, mesa..."
                  className="w-full rounded-md border px-3 py-2"
                />
                <button
                  type="button"
                  onClick={() => setSearch("")}
                  className="rounded-md border px-3 py-2 hover:bg-gray-50 disabled:opacity-50"
                  disabled={!search.trim()}
                >
                  Limpar
                </button>
              </div>
              <div className="mt-2 text-xs text-gray-500">
                Filtra por nome e descrição com debounce de 250ms.
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700">Ordenar por</label>
              <select
                value={sort}
                onChange={(event) => setSort(event.target.value as typeof sort)}
                className="mt-1 w-full rounded-md border bg-white px-3 py-2"
              >
                <option value="name">Nome (A–Z)</option>
                <option value="priceAsc">Preço (menor → maior)</option>
                <option value="priceDesc">Preço (maior → menor)</option>
              </select>

              <div className="mt-2 text-sm text-gray-600">
                {sorted.length} produto(s){normalizedSearch ? " (filtrado)" : ""}
              </div>
            </div>
          </div>
        </div>

        {loading && <div className="p-4">Carregando...</div>}

        {error && (
          <div className="p-4 text-red-700">
            Erro: {error}
            <div className="mt-2 text-sm text-gray-600">
              Confirme se a API está rodando em <code>{API_URL}</code>.
            </div>
          </div>
        )}

        {!loading && !error && sorted.length === 0 && (
          <div className="p-4 text-gray-600">Nenhum produto encontrado.</div>
        )}

        {!loading && !error && sorted.length > 0 && (
          <>
            <div className="p-4">
              <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
                {paged.map((product) => (
                  <Link
                    key={product.id}
                    href={`/catalog/${params.tenantSlug}/products/${product.id}`}
                    className="block rounded-lg border bg-white transition hover:shadow-sm"
                  >
                    <div className="space-y-2 p-4">
                      <div className="flex items-start justify-between gap-3">
                        <div className="font-semibold leading-snug">
                          <Highlight text={product.name} query={debouncedSearch} />
                        </div>
                        <div className="whitespace-nowrap text-sm font-bold">
                          {formatBRL(product.price)}
                        </div>
                      </div>

                      {product.description ? (
                        <div className="line-clamp-3 text-sm text-gray-600">
                          <Highlight text={product.description} query={debouncedSearch} />
                        </div>
                      ) : (
                        <div className="text-sm italic text-gray-400">Sem descrição.</div>
                      )}

                      <div className="pt-2 text-xs text-gray-500">Ver detalhes →</div>
                    </div>
                  </Link>
                ))}
              </div>
            </div>

            <div className="flex items-center justify-between border-t px-4 py-3">
              <div className="text-sm text-gray-600">
                Página {safePage} de {totalPages}
              </div>

              <div className="flex items-center gap-2">
                <button
                  className="rounded-md border px-3 py-1 hover:bg-gray-50 disabled:opacity-50"
                  disabled={safePage <= 1}
                  onClick={() => setPage((current) => Math.max(1, current - 1))}
                >
                  Anterior
                </button>
                <button
                  className="rounded-md border px-3 py-1 hover:bg-gray-50 disabled:opacity-50"
                  disabled={safePage >= totalPages}
                  onClick={() => setPage((current) => Math.min(totalPages, current + 1))}
                >
                  Próxima
                </button>
              </div>
            </div>
          </>
        )}
      </section>
    </main>
  );
}
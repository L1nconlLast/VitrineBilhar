"use client";

import { useEffect, useState } from "react";
import toast from "react-hot-toast";

type ProductDto = {
  id: string;
  name: string;
  description: string | null;
  price: number;
  isActive: boolean;
  categoryId: string | null;
};

const API_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:8080";

export default function AdminProductsPage() {
  const [items, setItems] = useState<ProductDto[]>([]);
  const [loading, setLoading] = useState(true);

  const [name, setName] = useState("Produto Teste");
  const [description, setDescription] = useState("Criado no admin DEV");
  const [price, setPrice] = useState(99.9);
  const [isActive, setIsActive] = useState(true);

  async function reload() {
    setLoading(true);
    try {
      const res = await fetch(`${API_URL}/api/admin/products`);
      if (!res.ok) throw new Error(`HTTP ${res.status}`);
      setItems(await res.json());
    } catch (e: any) {
      toast.error(e?.message ?? "Erro ao carregar");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => { reload(); }, []);

  async function create() {
    try {
      const res = await fetch(`${API_URL}/api/admin/products`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ name, description, price, isActive, categoryId: null })
      });
      if (!res.ok) throw new Error(`HTTP ${res.status}`);
      toast.success("Criado!");
      await reload();
    } catch (e: any) {
      toast.error(e?.message ?? "Erro ao criar");
    }
  }

  async function remove(id: string) {
    if (!confirm("Deletar?")) return;
    try {
      const res = await fetch(`${API_URL}/api/admin/products/${id}`, { method: "DELETE" });
      if (!res.ok && res.status !== 204) throw new Error(`HTTP ${res.status}`);
      toast.success("Deletado!");
      await reload();
    } catch (e: any) {
      toast.error(e?.message ?? "Erro ao deletar");
    }
  }

  async function toggleActive(p: ProductDto) {
    try {
      const res = await fetch(`${API_URL}/api/admin/products/${p.id}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          name: p.name,
          description: p.description,
          price: p.price,
          isActive: !p.isActive,
          categoryId: p.categoryId
        })
      });

      if (!res.ok && res.status !== 204) throw new Error(`HTTP ${res.status}`);

      toast.success(p.isActive ? "Desativado!" : "Ativado!");
      await reload();
    } catch (e: any) {
      toast.error(e?.message ?? "Erro ao alterar status");
    }
  }

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold">Produtos (DEV)</h1>
        <p className="text-sm text-gray-600">Sem autenticação, so para testar UI + API.</p>
      </div>

      <div className="rounded-lg border bg-white p-4 space-y-3">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-3">
          <input className="rounded-md border px-3 py-2" value={name} onChange={(e) => setName(e.target.value)} />
          <input className="rounded-md border px-3 py-2" value={description} onChange={(e) => setDescription(e.target.value)} />
          <input className="rounded-md border px-3 py-2" type="number" value={price} onChange={(e) => setPrice(Number(e.target.value))} />
          <label className="flex items-center gap-2 text-sm">
            <input type="checkbox" checked={isActive} onChange={(e) => setIsActive(e.target.checked)} />
            Ativo
          </label>
        </div>

        <button onClick={create} className="rounded-md bg-black text-white px-4 py-2">
          Criar produto
        </button>
      </div>

      <div className="rounded-lg border bg-white">
        <div className="border-b px-4 py-2 font-semibold">Lista</div>
        {loading && <div className="p-4">Carregando...</div>}
        <ul className="divide-y">
          {items.map((p) => (
            <li key={p.id} className="px-4 py-3 flex items-center justify-between">
              <div>
                <div className="font-medium">{p.name}</div>
                <div className="text-sm text-gray-600">
                  R$ {Number(p.price).toFixed(2)} • ativo: {p.isActive ? "sim" : "não"}
                </div>
              </div>
              <div className="flex items-center gap-2">
                <button
                  onClick={() => toggleActive(p)}
                  className="rounded-md border px-3 py-1 hover:bg-gray-50"
                  title="Alternar ativo/inativo"
                >
                  {p.isActive ? "Desativar" : "Ativar"}
                </button>

                <button onClick={() => remove(p.id)} className="rounded-md border px-3 py-1 hover:bg-gray-50">
                  Deletar
                </button>
              </div>
            </li>
          ))}
        </ul>
      </div>
    </div>
  );
}

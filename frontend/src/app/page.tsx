import Link from "next/link";

export default function Home() {
  return (
    <main className="mx-auto max-w-4xl px-4 py-10 space-y-6">
      <h1 className="text-3xl font-bold">VitrineBilhar</h1>

      <div className="space-y-2">
        <Link className="underline" href="/admin/products">
          Admin (DEV)
        </Link>
        <br />
        <Link className="underline" href="/catalog/demo">
          Catálogo público (demo)
        </Link>
      </div>
    </main>
  );
}

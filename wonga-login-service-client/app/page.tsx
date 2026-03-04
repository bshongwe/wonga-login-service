import Link from "next/link";

export default function Home() {
  return (
    <main className="min-h-screen flex flex-col items-center justify-center p-6">
      <h1 className="text-4xl font-bold mb-8">Welcome to Wonga Login Service</h1>
      <div className="space-x-4">
        <Link href="/login" className="px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700">
          Login
        </Link>
        <Link href="/register" className="px-6 py-3 bg-green-600 text-white rounded-lg hover:bg-green-700">
          Register
        </Link>
      </div>
    </main>
  );
}

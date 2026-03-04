import Link from "next/link";

export default function Home() {
  return (
    <div className="flex items-center justify-center min-h-[calc(100vh-8rem)]">
      <div className="w-full max-w-md sm:max-w-lg md:max-w-2xl">
        <div className="bg-white rounded-lg shadow-md p-6 sm:p-8 md:p-10">
          <h1 className="text-2xl sm:text-3xl md:text-4xl font-bold mb-3 sm:mb-4 text-gray-900">
            Welcome to Wonga Login Service
          </h1>
          <p className="text-sm sm:text-base text-gray-600 mb-6 sm:mb-8">
            Secure authentication platform with modern design and seamless user experience.
          </p>
          <div className="flex flex-col sm:flex-row gap-3 sm:gap-4">
            <Link 
              href="/login" 
              className="w-full sm:w-auto text-center px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors font-medium"
            >
              Login
            </Link>
            <Link 
              href="/register" 
              className="w-full sm:w-auto text-center px-6 py-3 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors font-medium"
            >
              Register
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
}

"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "next/navigation";
import Link from "next/link";
import Image from "next/image";
import { Input } from "@/components/ui/Input";
import { Button } from "@/components/ui/Button";
import { loginSchema, LoginFormData } from "@/lib/schemas";
import { authApi, ApiError } from "@/lib/api";
import { useAuth } from "@/contexts/AuthContext";

export default function LoginPage() {
  const router = useRouter();
  const { setUser } = useAuth();
  const [error, setError] = useState<string>("");
  const [isLoading, setIsLoading] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
  });

  const onSubmit = async (data: LoginFormData) => {
    try {
      setIsLoading(true);
      setError("");

      const response = await authApi.login(data);
      setUser(response.user);
      router.push("/user-details");
    } catch (err) {
      if (err instanceof ApiError) {
        setError(err.message);
      } else {
        setError("An unexpected error occurred");
      }
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="max-w-md mx-auto">
      <div
        className="bg-white rounded-lg shadow-md p-8"
      >
        <div className="flex items-center justify-center mb-6">
          <Image
            src="https://www.liblogo.com/img-logo/wo8579te50-wonga-logo-trending-stories-published-on-wonga-engineering-medium.png"
            alt="Wonga Logo"
            width={120}
            height={120}
            className="w-24 h-24 object-contain"
          />
        </div>

        <h1
          className="text-3xl font-bold text-center mb-2"
        >
          Login
        </h1>
        <p
          className="text-gray-600 text-center mb-6"
        >
          Welcome back! Please login to your account.
        </p>

        {error && (
          <div
            className="bg-red-50 border border-red-200 
            text-red-700 px-4 py-3 rounded mb-4"
          >
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <Input
            label="Email"
            type="email"
            placeholder="Enter your email"
            error={errors.email?.message}
            {...register("email")}
          />

          <Input
            label="Password"
            type="password"
            placeholder="Enter your password"
            error={errors.password?.message}
            {...register("password")}
          />

          <Button
            type="submit"
            variant="primary"
            isLoading={isLoading}
            className="w-full"
          >
            Login
          </Button>
        </form>

        <p className="text-center text-gray-600 mt-6">
          Don&apos;t have an account?{" "}
          <Link
            href="/register"
            className="text-blue-600 hover:underline"
          >
            Register here
          </Link>
        </p>
      </div>
    </div>
  );
}

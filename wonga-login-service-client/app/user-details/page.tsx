"use client";

import { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import { User, Mail, Calendar, LogOut } from "lucide-react";
import { Button } from "@/components/ui/Button";
import { authApi, UserResponse, ApiError } from "@/lib/api";

export default function UserDetailsPage() {
  const router = useRouter();
  const [user, setUser] = useState<UserResponse | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string>("");

  useEffect(() => {
    const fetchUserDetails = async () => {
      try {
        const token = localStorage.getItem("token");
        const storedUser = localStorage.getItem("user");

        if (!token) {
          router.push("/login");
          return;
        }

        if (storedUser) {
          setUser(JSON.parse(storedUser));
          setLoading(false);
        } else {
          const userData = await authApi.getUserDetails(token);
          setUser(userData);
          localStorage.setItem("user", JSON.stringify(userData));
        }
      } catch (err) {
        if (err instanceof ApiError) {
          setError(err.message);
          if (err.status === 401) {
            localStorage.removeItem("token");
            localStorage.removeItem("user");
            router.push("/login");
          }
        } else {
          setError("Failed to load user details");
        }
      } finally {
        setLoading(false);
      }
    };

    fetchUserDetails();
  }, [router]);

  const handleLogout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    router.push("/login");
  };

  if (loading) {
    return (
      <div className="max-w-2xl mx-auto">
        <div className="bg-white rounded-lg shadow-md p-8">
          <div
            className="flex items-center justify-center h-64"
          >
            <div className="text-gray-500">Loading...</div>
          </div>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="max-w-2xl mx-auto">
        <div className="bg-white rounded-lg shadow-md p-8">
          <div
            className="bg-red-50 border border-red-200 
            text-red-700 px-4 py-3 rounded"
          >
            {error}
          </div>
        </div>
      </div>
    );
  }

  if (!user) {
    return null;
  }

  return (
    <div className="max-w-2xl mx-auto">
      <div
        className="bg-white rounded-lg shadow-md p-8"
      >
        <div className="flex items-center justify-between mb-6">
          <h1 className="text-3xl font-bold">User Details</h1>
          <Button
            variant="danger"
            onClick={handleLogout}
            className="flex items-center space-x-2"
          >
            <LogOut size={18} />
            <span>Logout</span>
          </Button>
        </div>

        <div className="space-y-6">
          <div
            className="flex items-start space-x-4 p-4 
            bg-gray-50 rounded-lg"
          >
            <User
              className="w-6 h-6 text-blue-600 mt-1"
            />
            <div>
              <p className="text-sm text-gray-500">Username</p>
              <p className="text-lg font-medium">
                {user.username}
              </p>
            </div>
          </div>

          <div
            className="flex items-start space-x-4 p-4 
            bg-gray-50 rounded-lg"
          >
            <Mail
              className="w-6 h-6 text-blue-600 mt-1"
            />
            <div>
              <p className="text-sm text-gray-500">Email</p>
              <p className="text-lg font-medium">{user.email}</p>
            </div>
          </div>

          <div
            className="flex items-start space-x-4 p-4 
            bg-gray-50 rounded-lg"
          >
            <Calendar
              className="w-6 h-6 text-blue-600 mt-1"
            />
            <div>
              <p className="text-sm text-gray-500">
                Member Since
              </p>
              <p className="text-lg font-medium">
                {new Date(user.createdAt).toLocaleDateString()}
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

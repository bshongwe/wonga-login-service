import { NextResponse } from "next/server";
import { cookies } from "next/headers";

export async function GET() {
  const cookieStore = cookies();
  const token = cookieStore.get("auth_token")?.value;

  if (!token) {
    return NextResponse.json({ user: null }, { status: 401 });
  }

  try {
    // Call backend to validate token and get user details
    const backendUrl = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000/api";
    const response = await fetch(`${backendUrl}/user/details`, {
      headers: {
        Cookie: `auth_token=${token}`,
      },
    });

    if (!response.ok) {
      return NextResponse.json({ user: null }, { status: 401 });
    }

    const user = await response.json();
    return NextResponse.json({ user });
  } catch (error) {
    console.error("Session validation error:", error);
    return NextResponse.json({ user: null }, { status: 500 });
  }
}

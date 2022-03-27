Shader "Custom/CRT"
{
	SubShader
	{
		ZTest Always
		Cull Off
		ZWrite Off
		Fog {Mode Off}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;

			struct Attributes
			{
				float4 positionOS : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct Varyings
			{
				float4 pos: SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			Varyings vert(Attributes IN)
			{
				Varyings OUT;
				OUT.pos = UnityObjectToClipPos(IN.positionOS);
				OUT.uv = IN.uv;
				return OUT;
			}

			// 2�����x�N�g�����V�[�h�Ƃ���0~1�̃����_���l��Ԃ�
			float rand(float2 co)
			{
				return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43756.5453);
			}


			// ��ʂ��o�������Ă���悤�ɂ䂪�܂���
			float2 distort(float2 uv, float rate)
			{
				uv -= 0.5;
				uv /= 1 - length(uv) * rate;
				uv += 0.5;
				return uv;
			}

			// 3x3�̃K�E�V�A���t�B���^��������
			half4 gaussian_sample(float2 uv, float2 dx, float2 dy)
			{
				half4 col = 0;
				//col = tex2D(_MainTex, uv);
				col += tex2D(_MainTex, uv - dx - dy) * 1 / 16;
				col += tex2D(_MainTex, uv - dx) * 2 / 16;
				col += tex2D(_MainTex, uv - dx + dy) * 1 / 16;
				col += tex2D(_MainTex, uv - dy) * 2 / 16;
				col += tex2D(_MainTex, uv) * 4 / 16;
				col += tex2D(_MainTex, uv + dy) * 2 / 16;
				col += tex2D(_MainTex, uv + dx - dy) * 1 / 16;
				col += tex2D(_MainTex, uv + dx) * 2 / 16;
				col += tex2D(_MainTex, uv + dx + dy) * 1 / 16;
				return col;
			}

			// easing
			// �Q�l: https://easings.net/#easeInOutCubic
			float ease_in_out_cubic(const float x)
			{
				return x < 0.5
					? 4 * x * x * x
					: 1 - pow(-2 * x + 2, 3) / 2;
			}

			// CRT��1��f�̏㉺�[���Â��Ȃ錻�ۂ��Č�����
			float crt_ease(const float x, const float base, const float offset)
			{
				float tmp = fmod(x + offset, 1);
				float xx = 1 - abs(tmp * 2 - 1);
				float ease = ease_in_out_cubic(xx);
				return ease * base + base * 0.8;
			}


			fixed4 frag(Varyings IN) : SV_Target
			{
				float2 uv = IN.uv;

				// uv����ʂ��o�����Ă���悤�ɂ䂪�܂���
				uv = distort(uv, 0.2);

				// uv���͈͓��o�Ȃ���΍����h��Ԃ�
				if (uv.x < 0 || 1 < uv.x || uv.y < 0 || 1 < uv.y)
				{
					return float4(0, 0, 0, 1);
				}

				// ���݂̃s�N�Z���̐F��RGB�̂ǂꂩ
				// x�����������邱�ƂŁA
				// 1. �c�����ɓ����F�̉�f������
				// ��B������
				const float floor_x = fmod(IN.uv.x * _ScreenParams.x / 3, 1);
				const float isR = floor_x <= 0.3;
				const float isG = 0.3 < floor_x && floor_x <= 0.6;
				const float isB = 0.6 < floor_x;

				// �ׂ̃s�N�Z���܂ł�UV���W�ł̍����v�Z���Ă���
				const float2 dx = float2(1 / _ScreenParams.x, 0);
				const float2 dy = float2(0, 1 / _ScreenParams.y);

				// RGB���Ƃ�UV�����炷���ƂŁA
				// 3. ��f�̕��т͐F���ƂɈقȂ�I�t�Z�b�g������
				// ��B������
				uv += isR * -1 * dy;
				uv += isG * 0 * dy;
				uv += isB * 1 * dy;

				// �K�E�V�A���t�B���^�ɂ���āA���E���ڂ���
				// ���ɁA���w�i�Ƀh�b�g�G������������ł���悤�ȏꍇ��
				// �w�i�ƃI�u�W�F�N�g���n�b�L��������Ă��܂����Ƃ�h���ł���
				half4 col = gaussian_sample(uv, dx, dy);

				// �c������N�s�N�Z�����Ƃɕ������Ē[���Â����鏈���������邱�ƂŁA
				// 2. �㉺�̉�f�̒��Ԃɂ͈Â��Ȃ�̈悪���݂���
				// 4. �Â������ł͉�f�̑傫�����������Ȃ�
				// �𓯎��ɒB������
				const float floor_y = fmod(uv.y * _ScreenParams.y / 6, 1);
				const float ease_r = crt_ease(floor_y, col.r, rand(uv)* 0.1);
				const float ease_g = crt_ease(floor_y, col.g, rand(uv)* 0.1);
				const float ease_b = crt_ease(floor_y, col.b, rand(uv)* 0.1);

				// ���݂̃s�N�Z���ɂ����RGB�̂�����̐F������\������
				float r = isR * ease_r;
				float g = isG * ease_g;
				float b = isB * ease_b;

				return half4(r, g, b, 1);
			}
			ENDCG
		}
	}
}

#pragma once
// credits https://github.com/Urban82/Aes256
#include <cstdint>
#include <vector>
#include <windows.h>
#include <wincrypt.h>
#include <string>

using ByteArray = std::vector<uint8_t>;

#define BLOCK_SIZE 16


// AES256 implementation.
class Aes256
{

public:
	Aes256(const ByteArray& key);
	~Aes256();

	static ByteArray::size_type encrypt(const ByteArray& key, const ByteArray& plain, ByteArray& encrypted);
	static ByteArray::size_type encrypt(const ByteArray& key, const unsigned char* plain, const ByteArray::size_type plain_length, ByteArray& encrypted);
	static ByteArray::size_type decrypt(const ByteArray& key, const ByteArray& encrypted, ByteArray& plain);
	static ByteArray::size_type decrypt(const ByteArray& key, const unsigned char* encrypted, const ByteArray::size_type encrypted_length, ByteArray& plain);

	ByteArray::size_type encrypt_start(const ByteArray::size_type plain_length, ByteArray& encrypted);
	ByteArray::size_type encrypt_continue(const ByteArray& plain, ByteArray& encrypted);
	ByteArray::size_type encrypt_continue(const unsigned char* plain, const ByteArray::size_type plain_length, ByteArray& encrypted);
	ByteArray::size_type encrypt_end(ByteArray& encrypted);

	ByteArray::size_type decrypt_start(const ByteArray::size_type encrypted_length);
	ByteArray::size_type decrypt_continue(const ByteArray& encrypted, ByteArray& plain);
	ByteArray::size_type decrypt_continue(const unsigned char* encrypted, const ByteArray::size_type encrypted_length, ByteArray& plain);
	ByteArray::size_type decrypt_end(ByteArray& plain);

private:
	ByteArray            m_key;
	ByteArray            m_salt;
	ByteArray            m_rkey;

	unsigned char        m_buffer[3 * BLOCK_SIZE];
	unsigned char        m_buffer_pos;
	ByteArray::size_type m_remainingLength;

	bool                 m_decryptInitialized;

	void check_and_encrypt_buffer(ByteArray& encrypted);
	void check_and_decrypt_buffer(ByteArray& plain);

	void encrypt(unsigned char* buffer);
	void decrypt(unsigned char* buffer);

	void expand_enc_key(unsigned char* rc);
	void expand_dec_key(unsigned char* rc);

	void sub_bytes(unsigned char* buffer);
	void sub_bytes_inv(unsigned char* buffer);

	void copy_key();

	void add_round_key(unsigned char* buffer, const unsigned char round);

	void shift_rows(unsigned char* buffer);
	void shift_rows_inv(unsigned char* buffer);

	void mix_columns(unsigned char* buffer);
	void mix_columns_inv(unsigned char* buffer);
};

// Encryption wrapper.
class Encryption
{
	uint8_t    m_EncryptionKey[32];
	HCRYPTPROV m_CryptProvider;

public:
	// Generate a random cryptographic key.
	// OPTIONAL: You can pass a premade encryption key as a parameter.
	void Start();
	void Start(ByteArray& EncryptionKey);

	// Handles encryption/decryption of data.
	ByteArray Encrypt(ByteArray& Data);
	ByteArray Encrypt(ByteArray& Data, ByteArray key);
	ByteArray Decrypt(ByteArray& Data);
	ByteArray EncryptText(std::string Text);
	ByteArray EncryptText(std::string Text, ByteArray Key);
	std::string DecryptText(ByteArray& Data);
	std::string DecryptText(ByteArray& Data, ByteArray Key);
	// Exposes the encryption key.
	ByteArray GetKey()
	{
		ByteArray TemporaryKey;

		TemporaryKey.insert(
			TemporaryKey.begin(),
			m_EncryptionKey,
			m_EncryptionKey + sizeof m_EncryptionKey
		);

		return TemporaryKey;
	}
};
